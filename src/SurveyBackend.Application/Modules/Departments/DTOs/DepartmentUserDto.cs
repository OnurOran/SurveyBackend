namespace SurveyBackend.Application.Modules.Departments.DTOs;

public sealed record DepartmentUserDto(Guid Id, string Username, string Email, IReadOnlyCollection<string> Roles);
