namespace SurveyBackend.Domain.Departments;

public class Department
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string ExternalIdentifier { get; private set; } = null!;

    private Department()
    {
    }

    private Department(Guid id, string name, string externalIdentifier)
    {
        Id = id;
        Name = name;
        ExternalIdentifier = externalIdentifier;
    }

    public static Department Create(Guid id, string name, string externalIdentifier)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Department name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(externalIdentifier))
        {
            throw new ArgumentException("Department external identifier cannot be empty.", nameof(externalIdentifier));
        }

        return new Department(id, name.Trim(), externalIdentifier.Trim());
    }
}
