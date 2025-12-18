using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAttachmentService _attachmentService;
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // DOCX
            "application/msword", // DOC
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // XLSX
            "application/vnd.ms-excel", // XLS
            "application/vnd.openxmlformats-officedocument.presentationml.presentation", // PPTX
            "application/vnd.ms-powerpoint" // PPT
        };

    public AddQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        IAuthorizationService authorizationService,
        IAttachmentService attachmentService)
    {
        _surveyRepository = surveyRepository;
        _authorizationService = authorizationService;
        _attachmentService = attachmentService;
    }

    public async Task<int> HandleAsync(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        var questionDto = request.Question ?? throw new ArgumentNullException(nameof(request.Question));
        if (questionDto.Type == QuestionType.FileUpload && questionDto.Options is not null && questionDto.Options.Count > 0)
        {
            throw new InvalidOperationException("Dosya yükleme soruları için seçenek tanımlanamaz.");
        }
        if (questionDto.Type != QuestionType.FileUpload && questionDto.AllowedAttachmentContentTypes is not null && questionDto.AllowedAttachmentContentTypes.Count > 0)
        {
            throw new InvalidOperationException("Dosya tipi kısıtı sadece dosya yükleme soruları için geçerlidir.");
        }

        var question = survey.AddQuestion(questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
        if (questionDto.Type == QuestionType.FileUpload)
        {
            var normalizedAllowed = NormalizeAllowedContentTypes(questionDto.AllowedAttachmentContentTypes);
            question.SetAllowedAttachmentContentTypes(normalizedAllowed);
        }

        var optionAttachmentQueue = new List<(QuestionOption Option, AttachmentUploadDto Attachment)>();
        if (questionDto.Options is not null)
        {
            foreach (var option in questionDto.Options)
            {
                var createdOption = question.AddOption(option.Text, option.Order, option.Value);
                if (option.Attachment is not null)
                {
                    optionAttachmentQueue.Add((createdOption, option.Attachment));
                }
            }
        }

        await _surveyRepository.UpdateAsync(survey, cancellationToken);

        if (questionDto.Attachment is not null)
        {
            await _attachmentService.SaveQuestionAttachmentAsync(survey, question, questionDto.Attachment, cancellationToken);
        }

        foreach (var pair in optionAttachmentQueue)
        {
            await _attachmentService.SaveOptionAttachmentAsync(survey, pair.Option, pair.Attachment, cancellationToken);
        }
        return question.Id;
    }

    private static List<string>? NormalizeAllowedContentTypes(IEnumerable<string>? contentTypes)
    {
        if (contentTypes is null)
        {
            return null;
        }

        var list = contentTypes
            .Where(ct => !string.IsNullOrWhiteSpace(ct))
            .Select(ct => ct.Trim())
            .Where(ct => AllowedContentTypes.Contains(ct, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return list.Count > 0 ? list : null;
    }
}
