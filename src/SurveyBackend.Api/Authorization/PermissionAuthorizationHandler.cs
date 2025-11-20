using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace SurveyBackend.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string PermissionsClaimType = "permissions";
    private const string IndividualPermissionClaimType = "permission";
    private const string IsLocalAdminClaimType = "isLocalAdmin";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (IsLocalAdmin(context))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var directPermissions = context.User.FindAll(IndividualPermissionClaimType).Select(p => p.Value).ToArray();
        if (directPermissions.Length > 0 && requirement.IsSatisfiedBy(directPermissions))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var permissionsClaim = context.User.FindFirst(PermissionsClaimType);
        if (permissionsClaim is null || string.IsNullOrWhiteSpace(permissionsClaim.Value))
        {
            return Task.CompletedTask;
        }

        try
        {
            var grantedPermissions = ParsePermissions(permissionsClaim.Value);
            if (requirement.IsSatisfiedBy(grantedPermissions))
            {
                context.Succeed(requirement);
            }
        }
        catch (JsonException)
        {
            // Ignore malformed permission data, request will be rejected.
        }

        return Task.CompletedTask;
    }

    private static bool IsLocalAdmin(AuthorizationHandlerContext context)
    {
        var isLocalAdmin = context.User.FindFirst(IsLocalAdminClaimType)?.Value;
        return isLocalAdmin is not null && bool.TryParse(isLocalAdmin, out var result) && result;
    }

    private static IEnumerable<string> ParsePermissions(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return Array.Empty<string>();
        }

        var trimmed = rawValue.Trim();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            var parsed = JsonSerializer.Deserialize<string[]>(trimmed);
            return parsed ?? Array.Empty<string>();
        }

        return trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(permission => permission.Trim().Trim('"'));
    }
}
