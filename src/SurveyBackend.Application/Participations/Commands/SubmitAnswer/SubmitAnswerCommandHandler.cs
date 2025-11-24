using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandHandler : ICommandHandler<SubmitAnswerCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;

    public SubmitAnswerCommandHandler(IParticipationRepository participationRepository)
    {
        _participationRepository = participationRepository;
    }

    public async Task<bool> HandleAsync(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        // Load the participation with tracking enabled (default)
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        // Modify the tracked entity - EF Core will detect these changes
        participation.AddOrUpdateAnswer(Guid.NewGuid(), request.QuestionId, request.TextValue, request.OptionIds);

        // Save changes - EF Core automatically wraps this in a transaction
        await _participationRepository.UpdateAsync(participation, cancellationToken);

        return true;
    }
}
