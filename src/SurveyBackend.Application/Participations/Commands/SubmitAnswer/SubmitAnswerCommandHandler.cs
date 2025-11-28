using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandHandler : ICommandHandler<SubmitAnswerCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly AnswerAttachmentService _answerAttachmentService;

    public SubmitAnswerCommandHandler(
        IParticipationRepository participationRepository,
        ISurveyRepository surveyRepository,
        AnswerAttachmentService answerAttachmentService)
    {
        _participationRepository = participationRepository;
        _surveyRepository = surveyRepository;
        _answerAttachmentService = answerAttachmentService;
    }

    public async Task<bool> HandleAsync(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        // Load the participation with tracking enabled (default)
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        var survey = await _surveyRepository.GetByIdAsync(participation.SurveyId, cancellationToken)
                    ?? throw new InvalidOperationException("Anket bulunamadı.");

        var question = survey.Questions.FirstOrDefault(q => q.Id == request.QuestionId)
                       ?? throw new InvalidOperationException("Soru bulunamadı.");

        if (question.Type == QuestionType.FileUpload)
        {
            if (request.Attachment is null)
            {
                throw new InvalidOperationException("Bu soru için dosya yüklenmesi gereklidir.");
            }

            if (!string.IsNullOrWhiteSpace(request.TextValue) || (request.OptionIds is not null && request.OptionIds.Count > 0))
            {
                throw new InvalidOperationException("Dosya yükleme sorusu için metin veya seçenek gönderilemez.");
            }
        }
        else if (request.Attachment is not null)
        {
            throw new InvalidOperationException("Dosya yalnızca dosya yükleme tipi sorularda gönderilebilir.");
        }

        // Modify the tracked entity - EF Core will detect these changes
        var answer = participation.AddOrUpdateAnswer(
            Guid.NewGuid(),
            request.QuestionId,
            question.Type == QuestionType.FileUpload ? null : request.TextValue,
            question.Type == QuestionType.FileUpload ? null : request.OptionIds);

        // Save changes - EF Core automatically wraps this in a transaction
        await _participationRepository.UpdateAsync(participation, cancellationToken);

        if (question.Type == QuestionType.FileUpload && request.Attachment is not null)
        {
            await _answerAttachmentService.SaveAsync(survey, question, participation, answer, request.Attachment, cancellationToken);
        }

        return true;
    }
}
