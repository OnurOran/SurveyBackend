using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyDetailDto(
    Guid Id,
    string Title,
    string? Description,
    string? IntroText,
    string? ConsentText,
    string? OutroText,
    AccessType AccessType,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    IReadOnlyCollection<SurveyQuestionDetailDto> Questions,
    AttachmentDto? Attachment = null);

public sealed record SurveyQuestionDetailDto(
    Guid Id,
    string Text,
    string? Description,
    QuestionType Type,
    int Order,
    bool IsRequired,
    IReadOnlyCollection<SurveyOptionDetailDto> Options,
    AttachmentDto? Attachment = null,
    IReadOnlyCollection<string>? AllowedAttachmentContentTypes = null);

public sealed record SurveyOptionDetailDto(
    Guid Id,
    string Text,
    int Order,
    int? Value,
    AttachmentDto? Attachment = null,
    IReadOnlyCollection<SurveyQuestionDetailDto>? ChildQuestions = null);
