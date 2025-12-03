using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Commands.Update;

public sealed class UpdateSurveyCommandHandler : ICommandHandler<UpdateSurveyCommand, bool>
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
            "application/pdf"
        };

    public UpdateSurveyCommandHandler(
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

    public async Task<bool> HandleAsync(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                      ?? throw new InvalidOperationException("Anket bulunamadı.");

        if (existing.IsActive)
        {
            throw new InvalidOperationException("Yayınlanmış anketler düzenlenemez.");
        }

        var departmentId = existing.DepartmentId;
        await _authorizationService.EnsureDepartmentScopeAsync(departmentId, cancellationToken);

        // Build the replacement survey in-memory first (validation before destructive actions)
        var creator = existing.CreatedBy;
        var createdAt = existing.CreatedAt;

        var replacement = Survey.Create(
            request.SurveyId,
            request.Title,
            request.Description,
            request.IntroText,
            request.ConsentText,
            request.OutroText,
            creator,
            departmentId,
            request.AccessType,
            createdAt);

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

                if (questionDto.Type == Domain.Enums.QuestionType.Conditional)
                {
                    if (questionDto.Options is null || questionDto.Options.Count != 3)
                    {
                        throw new InvalidOperationException("Koşullu sorular tam olarak 3 seçenek içermelidir.");
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

                var question = replacement.AddQuestion(Guid.NewGuid(), questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
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
                    var option = question.AddOption(Guid.NewGuid(), optionDto.Text, optionDto.Order, optionDto.Value);
                    if (optionDto.Attachment is not null)
                    {
                        optionAttachmentQueue.Add((option, optionDto.Attachment));
                    }
                }

                if (questionDto.Type == Domain.Enums.QuestionType.Conditional && questionDto.ChildQuestions is not null)
                {
                    foreach (var childDto in questionDto.ChildQuestions)
                    {
                        var parentOption = question.Options.FirstOrDefault(o => o.Order == childDto.ParentOptionOrder);
                        if (parentOption is null)
                        {
                            throw new InvalidOperationException($"Seçenek sırası {childDto.ParentOptionOrder} bulunamadı.");
                        }

                        var childQuestion = replacement.AddQuestion(Guid.NewGuid(), childDto.Text, childDto.Type, childDto.Order, childDto.IsRequired);
                        if (childDto.Type == Domain.Enums.QuestionType.FileUpload)
                        {
                            var normalizedAllowed = NormalizeAllowedContentTypes(childDto.AllowedAttachmentContentTypes);
                            childQuestion.SetAllowedAttachmentContentTypes(normalizedAllowed);
                        }
                        if (childDto.Attachment is not null)
                        {
                            questionAttachmentQueue.Add((childQuestion, childDto.Attachment));
                        }

                        if (childDto.Options is not null)
                        {
                            foreach (var childOptionDto in childDto.Options)
                            {
                                var childOption = childQuestion.AddOption(Guid.NewGuid(), childOptionDto.Text, childOptionDto.Order, childOptionDto.Value);
                                if (childOptionDto.Attachment is not null)
                                {
                                    optionAttachmentQueue.Add((childOption, childOptionDto.Attachment));
                                }
                            }
                        }

                        parentOption.AddDependentQuestion(Guid.NewGuid(), childQuestion.Id);
                    }
                }
            }
        }

        // Now that new data is valid, remove old survey + attachments then persist replacement
        var attachmentsToRemove = CollectAttachments(existing);
        await _attachmentService.RemoveAttachmentsAsync(attachmentsToRemove, cancellationToken);
        await _surveyRepository.DeleteAsync(existing, cancellationToken);
        await _surveyRepository.AddAsync(replacement, cancellationToken);

        if (request.Attachment is not null)
        {
            await _attachmentService.SaveSurveyAttachmentAsync(replacement, request.Attachment, cancellationToken);
        }

        foreach (var pair in questionAttachmentQueue)
        {
            await _attachmentService.SaveQuestionAttachmentAsync(replacement, pair.Question, pair.Attachment, cancellationToken);
        }

        foreach (var pair in optionAttachmentQueue)
        {
            await _attachmentService.SaveOptionAttachmentAsync(replacement, pair.Option, pair.Attachment, cancellationToken);
        }

        return true;
    }

    private static List<Domain.Surveys.Attachment> CollectAttachments(Survey survey)
    {
        var list = new List<Domain.Surveys.Attachment>();

        if (survey.Attachment is not null)
        {
            list.Add(survey.Attachment);
        }

        foreach (var question in survey.Questions)
        {
            if (question.Attachment is not null)
            {
                list.Add(question.Attachment);
            }

            foreach (var option in question.Options)
            {
                if (option.Attachment is not null)
                {
                    list.Add(option.Attachment);
                }

                foreach (var dependent in option.DependentQuestions)
                {
                    if (dependent.ChildQuestion.Attachment is not null)
                    {
                        list.Add(dependent.ChildQuestion.Attachment);
                    }

                    foreach (var childOption in dependent.ChildQuestion.Options)
                    {
                        if (childOption.Attachment is not null)
                        {
                            list.Add(childOption.Attachment);
                        }
                    }
                }
            }
        }

        return list;
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
