using System.Collections.Generic;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IUserRoleRepository
{
    Task AssignRoleAsync(Guid userId, Guid roleId, Guid departmentId, CancellationToken cancellationToken);
    Task RemoveRoleAsync(Guid userId, Guid roleId, Guid departmentId, CancellationToken cancellationToken);
    Task RemoveAllRolesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetPermissionsForUserAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken);
    Task<bool> UserHasRoleAsync(Guid userId, string roleName, Guid departmentId, CancellationToken cancellationToken);
}
