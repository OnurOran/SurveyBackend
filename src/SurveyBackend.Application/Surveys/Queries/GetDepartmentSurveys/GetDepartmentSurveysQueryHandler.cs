using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentSurveys;

public sealed class GetDepartmentSurveysQueryHandler : ICommandHandler<GetDepartmentSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetDepartmentSurveysQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<IReadOnlyCollection<SurveyListItemDto>> HandleAsync(GetDepartmentSurveysQuery request, CancellationToken cancellationToken)
    {
        var surveys = await _surveyRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

        return surveys
            .Select(survey => new SurveyListItemDto(
                survey.Id,
                survey.Title,
                survey.Description,
                survey.DepartmentId,
                survey.AccessType,
                survey.IsActive,
                survey.CreatedAt,
                survey.StartDate,
                survey.EndDate,
                survey.CreatedBy))
            .ToList();
    }
}
