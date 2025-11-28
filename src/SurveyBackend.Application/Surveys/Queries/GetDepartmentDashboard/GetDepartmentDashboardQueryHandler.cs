using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentDashboard;

public sealed class GetDepartmentDashboardQueryHandler : ICommandHandler<GetDepartmentDashboardQuery, DepartmentDashboardDto>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetDepartmentDashboardQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<DepartmentDashboardDto> HandleAsync(GetDepartmentDashboardQuery request, CancellationToken cancellationToken)
    {
        var surveyStats = await _surveyRepository.GetSurveyStatsByDepartmentAsync(request.DepartmentId, cancellationToken);

        var totalSurveys = surveyStats.Count;
        var activeSurveys = surveyStats.Count(s => s.IsActive);
        var totalParticipations = surveyStats.Sum(s => s.ParticipationCount);

        var items = surveyStats
            .Select(s => new SurveyDashboardItemDto(s.SurveyId, s.Title, s.IsActive, s.CreatedAt, s.ParticipationCount))
            .ToList();

        return new DepartmentDashboardDto(
            request.DepartmentId,
            totalSurveys,
            activeSurveys,
            totalParticipations,
            items);
    }
}
