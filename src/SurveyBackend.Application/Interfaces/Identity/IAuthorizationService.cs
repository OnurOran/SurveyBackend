using System;

namespace SurveyBackend.Application.Interfaces.Identity;

public interface IAuthorizationService
{
    Task EnsurePermissionAsync(string permissionName, CancellationToken cancellationToken);
    Task EnsureDepartmentScopeAsync(Guid departmentId, CancellationToken cancellationToken);
    Task EnsureRoleManagementAsync(Guid departmentId, CancellationToken cancellationToken);
}
