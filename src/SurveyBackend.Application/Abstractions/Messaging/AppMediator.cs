using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SurveyBackend.Application.Behaviors;

namespace SurveyBackend.Application.Abstractions.Messaging;

public sealed class AppMediator : IAppMediator
{
    private readonly IServiceProvider _serviceProvider;

    public AppMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>
    {
        var validators = _serviceProvider.GetServices<IValidator<TCommand>>();
        await ValidationBehavior.ValidateAsync(validators, command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellationToken);
    }
}
