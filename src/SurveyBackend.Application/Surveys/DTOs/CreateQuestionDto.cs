using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record CreateQuestionDto(
    string Text,
    QuestionType Type,
    int Order,
    bool IsRequired,
    List<CreateOptionDto>? Options,
    AttachmentUploadDto? Attachment = null,
    List<string>? AllowedAttachmentContentTypes = null,
    List<CreateChildQuestionDto>? ChildQuestions = null);

public sealed record CreateChildQuestionDto(
    int ParentOptionOrder,
    string Text,
    QuestionType Type,
    int Order,
    bool IsRequired,
    List<CreateOptionDto>? Options,
    AttachmentUploadDto? Attachment = null,
    List<string>? AllowedAttachmentContentTypes = null);
