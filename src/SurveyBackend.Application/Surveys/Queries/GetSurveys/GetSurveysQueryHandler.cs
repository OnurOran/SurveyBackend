using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveys;

public sealed class GetSurveysQueryHandler : ICommandHandler<GetSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;

    public GetSurveysQueryHandler(ISurveyRepository surveyRepository, IUserRepository userRepository)
    {
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<SurveyListItemDto>> HandleAsync(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        var surveys = await _surveyRepository.GetAllAsync(cancellationToken);

        var creatorCache = new Dictionary<Guid, string>();

        var result = new List<SurveyListItemDto>(surveys.Count);
        foreach (var survey in surveys)
        {
            var createdBy = await ResolveCreatorAsync(survey.CreatedBy, creatorCache, cancellationToken);

            result.Add(new SurveyListItemDto(
                survey.Id,
                survey.Title,
                survey.Description,
                survey.DepartmentId,
                survey.AccessType,
                survey.IsActive,
                survey.CreatedAt,
                survey.StartDate,
                survey.EndDate,
                createdBy));
        }

        return result;
    }

    private async Task<string> ResolveCreatorAsync(string createdByValue, Dictionary<Guid, string> cache, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(createdByValue, out var userId))
        {
            return createdByValue;
        }

        if (cache.TryGetValue(userId, out var cached))
        {
            return cached;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var resolved = user?.Username ?? createdByValue;
        cache[userId] = resolved;
        return resolved;
    }
}
