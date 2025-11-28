using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentSurveys;

public sealed record GetDepartmentSurveysQuery(Guid DepartmentId) : ICommand<IReadOnlyCollection<SurveyListItemDto>>, IDepartmentScopedCommand;
