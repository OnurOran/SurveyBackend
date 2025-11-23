using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed class CompleteParticipationCommandHandler : ICommandHandler<CompleteParticipationCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteParticipationCommandHandler(IParticipationRepository participationRepository, IUnitOfWork unitOfWork)
    {
        _participationRepository = participationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(CompleteParticipationCommand request, CancellationToken cancellationToken)
    {
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        participation.Complete(DateTimeOffset.UtcNow);

        await _unitOfWork.ExecuteAsync(ct => _participationRepository.UpdateAsync(participation, ct), cancellationToken);
        return true;
    }
}
