using SurveyBackend.Application.Modules.Auth.Commands.Admin;

namespace SurveyBackend.Application.Modules.Auth.Validators;

public sealed class UpdateLocalAdminPasswordCommandValidator : AbstractValidator<UpdateLocalAdminPasswordCommand>
{
    public UpdateLocalAdminPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }
}
