using System.DirectoryServices;
using System.Linq;
using SurveyBackend.Application.Common.Models;
using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Directory;

public sealed class LdapService : ILdapService
{
    private readonly LdapSettings _settings;

    public LdapService(IOptions<LdapSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult<LdapUserInfo?>(null);
        }

        return Task.Run(() => AuthenticateInternal(username, password), cancellationToken);
    }

    private LdapUserInfo? AuthenticateInternal(string username, string password)
    {
        try
        {
            using var entry = new DirectoryEntry(_settings.Path, $"{_settings.Domain}\\{username}", password);
            using var searcher = new DirectorySearcher(entry)
            {
                Filter = $"(&({_settings.UsernameAttribute}={username})(objectClass=user))"
            };

            searcher.PropertiesToLoad.Add(_settings.UsernameAttribute);
            searcher.PropertiesToLoad.Add(_settings.EmailAttribute);
            searcher.PropertiesToLoad.Add(_settings.DepartmentAttribute);
            searcher.PropertiesToLoad.Add("objectGuid");

            var result = searcher.FindOne();
            if (result is null)
            {
                return null;
            }

            var email = result.Properties[_settings.EmailAttribute]?.Cast<object>().FirstOrDefault()?.ToString() ?? string.Empty;
            var department = result.Properties[_settings.DepartmentAttribute]?.Cast<object>().FirstOrDefault()?.ToString() ?? string.Empty;
            var objectGuid = result.Properties["objectGuid"]?.Cast<byte[]>().FirstOrDefault();
            var userId = objectGuid is not null ? new Guid(objectGuid) : Guid.NewGuid();

            return new LdapUserInfo(userId, username, email, department);
        }
        catch
        {
            return null;
        }
    }
}
