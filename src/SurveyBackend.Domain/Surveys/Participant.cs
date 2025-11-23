namespace SurveyBackend.Domain.Surveys;

public class Participant
{
    public Guid Id { get; private set; }
    public Guid? ExternalId { get; private set; }
    public string? LdapUsername { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Participant()
    {
    }

    private Participant(Guid id, Guid? externalId, string? ldapUsername, DateTimeOffset createdAt)
    {
        Id = id;
        ExternalId = externalId;
        LdapUsername = ldapUsername;
        CreatedAt = createdAt;
    }

    public static Participant Create(Guid id, DateTimeOffset createdAt, Guid? externalId = null, string? ldapUsername = null)
    {
        if (externalId is null && string.IsNullOrWhiteSpace(ldapUsername))
        {
            throw new ArgumentException("Participant must have either an external id or an LDAP username.");
        }

        return new Participant(id, externalId, ldapUsername?.Trim(), createdAt);
    }
}
