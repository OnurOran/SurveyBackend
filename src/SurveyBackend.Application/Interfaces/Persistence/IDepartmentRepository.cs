using SurveyBackend.Domain.Departments;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IDepartmentRepository
{
    Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken);
    Task<Department> AddAsync(Department department, CancellationToken cancellationToken);
}
