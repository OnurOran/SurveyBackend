using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyListItemDto(
    Guid Id,
    string Title,
    string? Description,
    Guid DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string CreatedBy);
