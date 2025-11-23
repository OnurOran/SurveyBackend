using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Participations;

public sealed class ParticipationRepository : IParticipationRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public ParticipationRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Participation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .Include(p => p.Answers)
                .ThenInclude(a => a.SelectedOptions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Participation?> GetBySurveyAndParticipantAsync(Guid surveyId, Guid participantId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SurveyId == surveyId && p.ParticipantId == participantId, cancellationToken);
    }

    public async Task AddAsync(Participation participation, CancellationToken cancellationToken)
    {
        await _dbContext.Participations.AddAsync(participation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Participation participation, CancellationToken cancellationToken)
    {
        if (_dbContext.Entry(participation).State == EntityState.Detached)
        {
            _dbContext.Participations.Attach(participation);
        }

        _dbContext.Participations.Update(participation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
