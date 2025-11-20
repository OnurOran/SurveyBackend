using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Users;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Authorization;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public UserRoleRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        if (await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken))
        {
            return;
        }

        _dbContext.UserRoles.Add(new UserRole(userId, roleId));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        if (userRole is null)
        {
            return;
        }

        _dbContext.UserRoles.Remove(userRole);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.Permissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName, cancellationToken);
    }
}
