using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface ISurveyRepository
{
    Task AddAsync(Survey survey, CancellationToken cancellationToken);
    Task<Survey?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Survey survey, CancellationToken cancellationToken);
}
