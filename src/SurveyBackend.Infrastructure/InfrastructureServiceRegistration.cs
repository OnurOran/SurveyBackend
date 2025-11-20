using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Infrastructure.Configurations;
using SurveyBackend.Infrastructure.Directory;
using SurveyBackend.Infrastructure.Persistence;
using SurveyBackend.Infrastructure.Repositories;
using SurveyBackend.Infrastructure.Repositories.Authorization;
using SurveyBackend.Infrastructure.Repositories.Departments;
using SurveyBackend.Infrastructure.Seeding;
using SurveyBackend.Infrastructure.Security;

namespace SurveyBackend.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

        services.AddDbContext<SurveyBackendDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<LdapSettings>(configuration.GetSection("Ldap"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddSingleton<IDepartmentRepository, InMemoryDepartmentRepository>();
        services.AddScoped<ILdapService, LdapService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
