using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace SurveyBackend.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string PermissionsClaimType = "permissions";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissionsClaim = context.User.FindFirst(PermissionsClaimType);
        if (permissionsClaim is null || string.IsNullOrWhiteSpace(permissionsClaim.Value))
        {
            return Task.CompletedTask;
        }

        try
        {
            var grantedPermissions = JsonSerializer.Deserialize<string[]>(permissionsClaim.Value);
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
}
