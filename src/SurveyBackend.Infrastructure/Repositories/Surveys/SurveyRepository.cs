using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Surveys;

public sealed class SurveyRepository : ISurveyRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public SurveyRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Survey survey, CancellationToken cancellationToken)
    {
        await _dbContext.Surveys.AddAsync(survey, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Survey?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Surveys
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Survey survey, CancellationToken cancellationToken)
    {
        if (_dbContext.Entry(survey).State == EntityState.Detached)
        {
            _dbContext.Surveys.Attach(survey);
        }

        // Ensure the aggregate graph is tracked for updates that depend on the full hierarchy.
        var questionsCollection = _dbContext.Entry(survey).Collection(s => s.Questions);
        if (!questionsCollection.IsLoaded)
        {
            await questionsCollection.Query()
                .Include(q => q.Options)
                .AsSplitQuery()
                .LoadAsync(cancellationToken);
        }
        else
        {
            foreach (var question in survey.Questions)
            {
                var optionsCollection = _dbContext.Entry(question).Collection(q => q.Options);
                if (!optionsCollection.IsLoaded)
                {
                    await optionsCollection.LoadAsync(cancellationToken);
                }
            }
        }

        _dbContext.Surveys.Update(survey);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
