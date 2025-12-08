namespace SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

public sealed record CheckParticipationStatusQuery(Guid SurveyId, Guid? ExternalId) : ICommand<ParticipationStatusResult>;
