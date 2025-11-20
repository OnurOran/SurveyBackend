using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Validators;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        RuleFor(x => x.RoleId).NotEqual(Guid.Empty);
    }
}
