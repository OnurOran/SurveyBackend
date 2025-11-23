using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurvey;

public sealed record GetSurveyQuery(Guid Id) : ICommand<SurveyDetailDto?>;
