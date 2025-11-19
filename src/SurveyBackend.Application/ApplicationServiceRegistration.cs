using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly);
        RegisterCommandHandlers(services, assembly);
        services.AddScoped<IAppMediator, AppMediator>();

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(ICommandHandler<,>);
        var handlerRegistrations = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(@interface => new { Implementation = type, Service = @interface }));

        foreach (var registration in handlerRegistrations)
        {
            services.AddScoped(registration.Service, registration.Implementation);
        }
    }
}
