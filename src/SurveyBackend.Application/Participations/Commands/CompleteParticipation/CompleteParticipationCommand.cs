namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed record CompleteParticipationCommand(Guid ParticipationId) : ICommand<bool>;
