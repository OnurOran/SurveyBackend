namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed class StartParticipationCommandValidator : AbstractValidator<StartParticipationCommand>
{
    public StartParticipationCommandValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty();
    }
}
