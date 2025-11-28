namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record DepartmentDashboardDto(
    Guid DepartmentId,
    int TotalSurveys,
    int ActiveSurveys,
    int TotalParticipations,
    IReadOnlyCollection<SurveyDashboardItemDto> Surveys);
