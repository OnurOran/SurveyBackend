namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandValidator : AbstractValidator<SubmitAnswerCommand>
{
    private const int MaxTextAnswerLength = 2000;

    public SubmitAnswerCommandValidator()
    {
        RuleFor(x => x.ParticipationId)
            .NotEmpty();

        RuleFor(x => x.QuestionId)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.TextValue) || (x.OptionIds is not null && x.OptionIds.Any()))
            .WithMessage("At least one answer (text or options) must be provided.");

        RuleFor(x => x.TextValue)
            .MaximumLength(MaxTextAnswerLength)
            .WithMessage($"Text answer cannot exceed {MaxTextAnswerLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TextValue));
    }
}
