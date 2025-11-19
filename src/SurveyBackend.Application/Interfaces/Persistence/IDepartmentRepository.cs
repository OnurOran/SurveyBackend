using SurveyBackend.Domain.Departments;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IDepartmentRepository
{
    Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken);
}
