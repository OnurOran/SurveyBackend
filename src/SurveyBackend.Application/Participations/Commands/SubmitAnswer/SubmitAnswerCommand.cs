namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed record SubmitAnswerCommand(Guid ParticipationId, Guid QuestionId, string? TextValue, List<Guid>? OptionIds) : ICommand<bool>;
