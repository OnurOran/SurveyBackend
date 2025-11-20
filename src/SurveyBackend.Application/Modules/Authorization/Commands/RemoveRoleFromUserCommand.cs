namespace SurveyBackend.Application.Modules.Authorization.Commands;

public sealed record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : ICommand<bool>;
