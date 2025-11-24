namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed record StartParticipationCommand(Guid SurveyId, Guid? ExternalId) : ICommand<Guid>
{
    public string? IpAddress { get; init; }
}
