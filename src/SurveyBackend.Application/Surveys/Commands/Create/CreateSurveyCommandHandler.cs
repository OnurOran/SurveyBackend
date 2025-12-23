using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Commands.Create;

public sealed class CreateSurveyCommandHandler : ICommandHandler<CreateSurveyCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;
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

    public CreateSurveyCommandHandler(
        ISurveyRepository surveyRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IAttachmentService attachmentService)
    {
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _attachmentService = attachmentService;
    }

    public async Task<int> HandleAsync(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
        }

        var departmentId = _currentUserService.DepartmentId
            ?? throw new UnauthorizedAccessException("Kullanıcının departman bilgisi bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(departmentId, cancellationToken);

        // Generate unique slug for the survey
        var baseSlug = Survey.GenerateSlug(request.Title);
        var slug = baseSlug;
        var counter = 1;

        // Ensure slug uniqueness by appending counter if needed
        while (await _surveyRepository.SlugExistsAsync(slug, cancellationToken))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        var survey = Survey.Create(slug, request.Title, request.Description, request.IntroText, request.ConsentText, request.OutroText, departmentId, request.AccessType);

        var questionAttachmentQueue = new List<(Question Question, AttachmentUploadDto Attachment)>();
        var optionAttachmentQueue = new List<(QuestionOption Option, AttachmentUploadDto Attachment)>();

        if (request.Questions is not null)
        {
            foreach (var questionDto in request.Questions)
            {
                if (questionDto.Type == Domain.Enums.QuestionType.FileUpload && questionDto.Options is not null && questionDto.Options.Count > 0)
                {
                    throw new InvalidOperationException("Dosya yükleme soruları için seçenek tanımlanamaz.");
                }
                if (questionDto.Type != Domain.Enums.QuestionType.FileUpload && questionDto.AllowedAttachmentContentTypes is not null && questionDto.AllowedAttachmentContentTypes.Count > 0)
                {
                    throw new InvalidOperationException("Dosya tipi kısıtı sadece dosya yükleme soruları için geçerlidir.");
                }

                // Validate Conditional questions
                if (questionDto.Type == Domain.Enums.QuestionType.Conditional)
                {
                    if (questionDto.Options is null || questionDto.Options.Count < 2 || questionDto.Options.Count > 5)
                    {
                        throw new InvalidOperationException("Koşullu sorular 2 ile 5 arasında seçenek içermelidir.");
                    }
                    if (questionDto.ChildQuestions is not null)
                    {
                        foreach (var childDto in questionDto.ChildQuestions)
                        {
                            if (childDto.Type == Domain.Enums.QuestionType.Conditional)
                            {
                                throw new InvalidOperationException("Alt sorular Koşullu tip olamaz.");
                            }
                        }
                    }
                }

                // Validate SingleSelect/MultiSelect questions
                if (questionDto.Type == Domain.Enums.QuestionType.SingleSelect || questionDto.Type == Domain.Enums.QuestionType.MultiSelect)
                {
                    if (questionDto.Options is null || questionDto.Options.Count < 2 || questionDto.Options.Count > 10)
                    {
                        throw new InvalidOperationException("Tek seçim ve çoklu seçim soruları 2 ile 10 arasında seçenek içermelidir.");
                    }
                }

                var question = survey.AddQuestion(questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
                if (questionDto.Type == Domain.Enums.QuestionType.FileUpload)
                {
                    var normalizedAllowed = NormalizeAllowedContentTypes(questionDto.AllowedAttachmentContentTypes);
                    question.SetAllowedAttachmentContentTypes(normalizedAllowed);
                }
                if (questionDto.Attachment is not null)
                {
                    questionAttachmentQueue.Add((question, questionDto.Attachment));
                }
                if (questionDto.Options is null)
                {
                    continue;
                }

                foreach (var optionDto in questionDto.Options)
                {
                    var option = question.AddOption(optionDto.Text, optionDto.Order, optionDto.Value);
                    if (optionDto.Attachment is not null)
                    {
                        optionAttachmentQueue.Add((option, optionDto.Attachment));
                    }
                }

                // Handle child questions for Conditional questions
                if (questionDto.Type == Domain.Enums.QuestionType.Conditional && questionDto.ChildQuestions is not null)
                {
                    foreach (var childDto in questionDto.ChildQuestions)
                    {
                        // Find parent option by order
                        var parentOption = question.Options.FirstOrDefault(o => o.Order == childDto.ParentOptionOrder);
                        if (parentOption is null)
                        {
                            throw new InvalidOperationException($"Seçenek sırası {childDto.ParentOptionOrder} bulunamadı.");
                        }

                        // Create child question
                        var childQuestion = survey.AddQuestion(childDto.Text, childDto.Type, childDto.Order, childDto.IsRequired);
                        if (childDto.Type == Domain.Enums.QuestionType.FileUpload)
                        {
                            var normalizedAllowed = NormalizeAllowedContentTypes(childDto.AllowedAttachmentContentTypes);
                            childQuestion.SetAllowedAttachmentContentTypes(normalizedAllowed);
                        }
                        if (childDto.Attachment is not null)
                        {
                            questionAttachmentQueue.Add((childQuestion, childDto.Attachment));
                        }

                        // Add child question options if any
                        if (childDto.Options is not null)
                        {
                            foreach (var childOptionDto in childDto.Options)
                            {
                                var childOption = childQuestion.AddOption(childOptionDto.Text, childOptionDto.Order, childOptionDto.Value);
                                if (childOptionDto.Attachment is not null)
                                {
                                    optionAttachmentQueue.Add((childOption, childOptionDto.Attachment));
                                }
                            }
                        }

                        // Create dependent question mapping
                        parentOption.AddDependentQuestion(childQuestion.Id);
                    }
                }
            }
        }

        await _surveyRepository.AddAsync(survey, cancellationToken);

        if (request.Attachment is not null)
        {
            await _attachmentService.SaveSurveyAttachmentAsync(survey, request.Attachment, cancellationToken);
        }

        foreach (var pair in questionAttachmentQueue)
        {
            await _attachmentService.SaveQuestionAttachmentAsync(survey, pair.Question, pair.Attachment, cancellationToken);
        }

        foreach (var pair in optionAttachmentQueue)
        {
            await _attachmentService.SaveOptionAttachmentAsync(survey, pair.Option, pair.Attachment, cancellationToken);
        }

        return survey.Id;
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
