namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record PublishSurveyRequest(DateTimeOffset? StartDate, DateTimeOffset? EndDate);
