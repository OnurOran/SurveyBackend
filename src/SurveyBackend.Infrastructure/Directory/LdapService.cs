using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
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
        using var cn = new LdapConnection();

        try
        {
            cn.Connect(_settings.Path, 389);
            var bindDn = $"{username}@{_settings.Domain}";
            cn.Bind(bindDn, password);

            var searchFilter = $"(&({_settings.UsernameAttribute}={username})(objectClass=user))";
            var searchResults = cn.Search(
                _settings.SearchBase,
                2, // LdapConnection.ScopeSub = 2 (subtree search)
                searchFilter,
                new[] { _settings.EmailAttribute, _settings.DepartmentAttribute, "objectGuid" },
                false);

            if (searchResults.HasMore())
            {
                var entry = searchResults.Next();
                var email = entry.GetAttribute(_settings.EmailAttribute)?.StringValue ?? string.Empty;
                var department = entry.GetAttribute(_settings.DepartmentAttribute)?.StringValue ?? string.Empty;

                var userId = Guid.NewGuid();
                try
                {
                    var guidAttr = entry.GetAttribute("objectGuid");
                    if (guidAttr is not null)
                    {
                        userId = new Guid((byte[])(object)guidAttr.ByteValue);
                    }
                }
                catch
                {
                    // ignore parsing errors and keep generated id
                }

                return new LdapUserInfo(userId, username, email, department);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
