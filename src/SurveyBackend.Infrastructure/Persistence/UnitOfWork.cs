using Microsoft.EntityFrameworkCore.Storage;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SurveyBackendDbContext _dbContext;

    public UnitOfWork(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
