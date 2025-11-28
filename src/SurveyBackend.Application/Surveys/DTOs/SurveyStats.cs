namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyStats(Guid SurveyId, string Title, Guid DepartmentId, bool IsActive, DateTimeOffset CreatedAt, int ParticipationCount);
