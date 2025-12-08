using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed record GetSurveyReportQuery(Guid SurveyId) : ICommand<SurveyReportDto?>;

/// <summary>
/// Optional query to get individual participant responses
/// </summary>
public sealed record GetParticipantResponseQuery(Guid SurveyId, Guid ParticipationId) : ICommand<ParticipantResponseDto?>;
