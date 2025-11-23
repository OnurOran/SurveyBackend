using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyDetailDto(
    Guid Id,
    string Title,
    string? Description,
    AccessType AccessType,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    IReadOnlyCollection<SurveyQuestionDetailDto> Questions);

public sealed record SurveyQuestionDetailDto(
    Guid Id,
    string Text,
    string? Description,
    QuestionType Type,
    int Order,
    bool IsRequired,
    IReadOnlyCollection<SurveyOptionDetailDto> Options);

public sealed record SurveyOptionDetailDto(Guid Id, string Text, int Order, int? Value);
