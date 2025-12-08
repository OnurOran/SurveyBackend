using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

public sealed class CheckParticipationStatusQueryHandler : ICommandHandler<CheckParticipationStatusQuery, ParticipationStatusResult>
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public CheckParticipationStatusQueryHandler(
        IParticipantRepository participantRepository,
        IParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _participantRepository = participantRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ParticipationStatusResult> HandleAsync(CheckParticipationStatusQuery request, CancellationToken cancellationToken)
    {
        Domain.Surveys.Participant? participant = null;

        // Identify the participant (authenticated or anonymous)
        if (_currentUserService.IsAuthenticated)
        {
            var username = _currentUserService.Username;
            if (!string.IsNullOrWhiteSpace(username))
            {
                participant = await _participantRepository.GetByLdapUsernameAsync(username, cancellationToken);
            }
        }
        else
        {
            if (request.ExternalId.HasValue)
            {
                participant = await _participantRepository.GetByExternalIdAsync(request.ExternalId.Value, cancellationToken);
            }
        }

        // If participant doesn't exist, they haven't participated
        if (participant is null)
        {
            return new ParticipationStatusResult(
                HasParticipated: false,
                IsCompleted: false,
                CompletedAt: null);
        }

        // Check if participation exists for this survey and participant
        var participation = await _participationRepository.GetBySurveyAndParticipantAsync(
            request.SurveyId,
            participant.Id,
            cancellationToken);

        // If no participation exists
        if (participation is null)
        {
            return new ParticipationStatusResult(
                HasParticipated: false,
                IsCompleted: false,
                CompletedAt: null);
        }

        // Participation exists, check if completed
        var isCompleted = participation.CompletedAt.HasValue;
        return new ParticipationStatusResult(
            HasParticipated: true,
            IsCompleted: isCompleted,
            CompletedAt: participation.CompletedAt);
    }
}
