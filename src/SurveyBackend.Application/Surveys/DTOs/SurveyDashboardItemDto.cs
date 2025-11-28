namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyDashboardItemDto(
    Guid SurveyId,
    string Title,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int ParticipationCount);
