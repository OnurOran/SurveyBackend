using System;
using System.Collections.Concurrent;
using System.Linq;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Departments;

namespace SurveyBackend.Infrastructure.Repositories.Departments;

public sealed class InMemoryDepartmentRepository : IDepartmentRepository
{
    private static readonly ConcurrentDictionary<string, Department> _departments = new(StringComparer.OrdinalIgnoreCase);

    static InMemoryDepartmentRepository()
    {
        // Seed some initial departments
        _departments.TryAdd("hr", new Department(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Human Resources", "hr"));
        _departments.TryAdd("engineering", new Department(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Engineering", "engineering"));
        _departments.TryAdd("sales", new Department(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Sales", "sales"));
    }

    public Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken)
    {
        _departments.TryGetValue(externalIdentifier, out var department);
        return Task.FromResult(department);
    }

    public Task<Department> AddAsync(Department department, CancellationToken cancellationToken)
    {
        _departments.TryAdd(department.ExternalIdentifier, department);
        return Task.FromResult(department);
    }
}
