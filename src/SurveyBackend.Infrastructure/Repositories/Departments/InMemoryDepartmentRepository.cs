using System;
using System.Linq;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Departments;

namespace SurveyBackend.Infrastructure.Repositories.Departments;

public sealed class InMemoryDepartmentRepository : IDepartmentRepository
{
    private static readonly IReadOnlyList<Department> Departments = new List<Department>
    {
        new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Human Resources", "hr"),
        new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Engineering", "engineering"),
        new(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Sales", "sales")
    };

    public Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken)
    {
        var department = Departments.FirstOrDefault(d =>
            d.ExternalIdentifier.Equals(externalIdentifier, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(department);
    }
}
