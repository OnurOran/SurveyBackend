using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveys;

public sealed class GetSurveysQueryHandler : ICommandHandler<GetSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetSurveysQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<IReadOnlyCollection<SurveyListItemDto>> HandleAsync(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        var surveys = await _surveyRepository.GetAllAsync(cancellationToken);

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
