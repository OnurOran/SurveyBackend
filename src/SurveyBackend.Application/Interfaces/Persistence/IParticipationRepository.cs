using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IParticipationRepository
{
    Task<Participation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Participation?> GetBySurveyAndParticipantAsync(Guid surveyId, Guid participantId, CancellationToken cancellationToken);
    Task AddAsync(Participation participation, CancellationToken cancellationToken);
    Task UpdateAsync(Participation participation, CancellationToken cancellationToken);
}
