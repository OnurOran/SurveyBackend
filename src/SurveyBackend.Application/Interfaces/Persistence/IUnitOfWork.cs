namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);
}
