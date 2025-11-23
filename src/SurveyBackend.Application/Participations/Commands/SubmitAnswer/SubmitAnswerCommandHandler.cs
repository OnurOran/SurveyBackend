using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandHandler : ICommandHandler<SubmitAnswerCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitAnswerCommandHandler(IParticipationRepository participationRepository, IUnitOfWork unitOfWork)
    {
        _participationRepository = participationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        participation.AddOrUpdateAnswer(Guid.NewGuid(), request.QuestionId, request.TextValue, request.OptionIds);

        await _unitOfWork.ExecuteAsync(ct => _participationRepository.UpdateAsync(participation, ct), cancellationToken);
        return true;
    }
}
