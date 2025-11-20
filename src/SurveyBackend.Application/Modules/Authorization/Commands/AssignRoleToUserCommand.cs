namespace SurveyBackend.Application.Modules.Authorization.Commands;

public sealed record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : ICommand<bool>;
