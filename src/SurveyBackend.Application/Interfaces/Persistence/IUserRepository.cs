using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetLocalByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
}
