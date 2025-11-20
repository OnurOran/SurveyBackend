using System.Collections.Generic;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IUserRoleRepository
{
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetPermissionsForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken);
}
