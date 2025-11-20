namespace SurveyBackend.Application.Modules.Authorization.DTOs;

public sealed record RoleDto(Guid Id, string Name, string Description, IReadOnlyCollection<string> Permissions);
