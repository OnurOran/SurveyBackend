using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed class StartParticipationCommandHandler : ICommandHandler<StartParticipationCommand, Guid>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public StartParticipationCommandHandler(
        ISurveyRepository surveyRepository,
        IParticipantRepository participantRepository,
        IParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participantRepository = participantRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> HandleAsync(StartParticipationCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        if (!IsAvailable(survey))
        {
            throw new InvalidOperationException("Anket şu anda aktif değil.");
        }

        var now = DateTimeOffset.UtcNow;
        var isNewParticipant = false;
        Participant participant = null!;

        if (_currentUserService.IsAuthenticated)
        {
            var username = _currentUserService.Username;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("Kullanıcı adı bulunamadı.");
            }

            var existingParticipant = await _participantRepository.GetByLdapUsernameAsync(username, cancellationToken);
            if (existingParticipant is null)
            {
                participant = Participant.CreateInternal(Guid.NewGuid(), username, now);
                isNewParticipant = true;
            }
            else
            {
                participant = existingParticipant;
            }
        }
        else
        {
            if (!request.ExternalId.HasValue)
            {
                throw new InvalidOperationException("Anonim katılım için dış kimlik gereklidir.");
            }

            var externalId = request.ExternalId.Value;
            var existingParticipant = await _participantRepository.GetByExternalIdAsync(externalId, cancellationToken);
            if (existingParticipant is null)
            {
                participant = Participant.CreateAnonymous(Guid.NewGuid(), externalId, now);
                isNewParticipant = true;
            }
            else
            {
                participant = existingParticipant;
            }
        }

        var existingParticipation = await _participationRepository.GetBySurveyAndParticipantAsync(survey.Id, participant.Id, cancellationToken);
        if (existingParticipation is not null)
        {
            return existingParticipation.Id;
        }

        var participation = Participation.Start(Guid.NewGuid(), survey.Id, participant.Id, now, request.IpAddress);

        // Add participant if it's new (EF Core will handle this in a single transaction)
        if (isNewParticipant)
        {
            await _participantRepository.AddAsync(participant, cancellationToken);
        }

        await _participationRepository.AddAsync(participation, cancellationToken);

        return participation.Id;
    }

    private static bool IsAvailable(Survey survey)
    {
        var now = DateTimeOffset.UtcNow;

        if (!survey.IsActive)
        {
            return false;
        }

        if (survey.StartDate.HasValue && survey.StartDate.Value > now)
        {
            return false;
        }

        if (survey.EndDate.HasValue && survey.EndDate.Value <= now)
        {
            return false;
        }

        return true;
    }
}
