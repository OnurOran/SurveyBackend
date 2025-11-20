using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Validators;

public sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        RuleFor(x => x.RoleId).NotEqual(Guid.Empty);
    }
}
