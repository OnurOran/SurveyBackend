using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed class CompleteParticipationCommandHandler : ICommandHandler<CompleteParticipationCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;

    public CompleteParticipationCommandHandler(IParticipationRepository participationRepository)
    {
        _participationRepository = participationRepository;
    }

    public async Task<bool> HandleAsync(CompleteParticipationCommand request, CancellationToken cancellationToken)
    {
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        participation.Complete(DateTimeOffset.UtcNow);

        await _participationRepository.UpdateAsync(participation, cancellationToken);
        return true;
    }
}
