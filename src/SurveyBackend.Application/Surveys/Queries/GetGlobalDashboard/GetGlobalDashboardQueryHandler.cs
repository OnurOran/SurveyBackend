using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetGlobalDashboard;

public sealed class GetGlobalDashboardQueryHandler : ICommandHandler<GetGlobalDashboardQuery, GlobalDashboardDto>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetGlobalDashboardQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<GlobalDashboardDto> HandleAsync(GetGlobalDashboardQuery request, CancellationToken cancellationToken)
    {
        var surveyStats = await _surveyRepository.GetSurveyStatsAsync(cancellationToken);

        var totalSurveys = surveyStats.Count;
        var activeSurveys = surveyStats.Count(s => s.IsActive);
        var totalParticipations = surveyStats.Sum(s => s.ParticipationCount);

        var items = surveyStats
            .Select(s => new SurveyDashboardItemDto(s.SurveyId, s.Title, s.IsActive, s.CreatedAt, s.ParticipationCount))
            .ToList();

        return new GlobalDashboardDto(
            totalSurveys,
            activeSurveys,
            totalParticipations,
            items);
    }
}
