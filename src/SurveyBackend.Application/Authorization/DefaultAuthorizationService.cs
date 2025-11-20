using SurveyBackend.Application.Interfaces.Identity;

namespace SurveyBackend.Application.Authorization;

public sealed class DefaultAuthorizationService : IAuthorizationService
{
    private readonly ICurrentUserService _currentUserService;

    public DefaultAuthorizationService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Task EnsurePermissionAsync(string permissionName, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsLocalAdmin)
        {
            return Task.CompletedTask;
        }

        if (!_currentUserService.HasPermission(permissionName))
        {
            throw new UnauthorizedAccessException("Bu işlem için yetkiniz bulunmamaktadır.");
        }

        return Task.CompletedTask;
    }

    public Task EnsureDepartmentScopeAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsLocalAdmin)
        {
            return Task.CompletedTask;
        }

        if (!_currentUserService.DepartmentId.HasValue || _currentUserService.DepartmentId.Value != departmentId)
        {
            throw new UnauthorizedAccessException("Bu departman üzerinde işlem yapma yetkiniz yok.");
        }

        return Task.CompletedTask;
    }

    public Task EnsureRoleManagementAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsLocalAdmin || _currentUserService.HasPermission("ManageUsers"))
        {
            return Task.CompletedTask;
        }

        if (_currentUserService.HasPermission("ManageDepartment"))
        {
            if (!_currentUserService.DepartmentId.HasValue || _currentUserService.DepartmentId.Value != departmentId)
            {
                throw new UnauthorizedAccessException("Bu departman üzerinde işlem yapma yetkiniz yok.");
            }

            return Task.CompletedTask;
        }

        throw new UnauthorizedAccessException("Kullanıcı rolleri üzerinde işlem yapma yetkiniz yok.");
    }
}
