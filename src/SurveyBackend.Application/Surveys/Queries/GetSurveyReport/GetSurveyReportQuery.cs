using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed record GetSurveyReportQuery(int SurveyId) : ICommand<SurveyReportDto?>;

/// <summary>
/// Optional query to get individual participant responses
/// </summary>
public sealed record GetParticipantResponseQuery(int SurveyId, int ParticipationId) : ICommand<ParticipantResponseDto?>;
