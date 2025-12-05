using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IParticipationRepository
{
    Task<Participation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Participation?> GetBySurveyAndParticipantAsync(Guid surveyId, Guid participantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Participation>> GetBySurveyIdAsync(Guid surveyId, CancellationToken cancellationToken);
    Task<Participation?> GetByParticipantNameAsync(Guid surveyId, string participantName, CancellationToken cancellationToken);
    Task AddAsync(Participation participation, CancellationToken cancellationToken);
    Task UpdateAsync(Participation participation, CancellationToken cancellationToken);
}
