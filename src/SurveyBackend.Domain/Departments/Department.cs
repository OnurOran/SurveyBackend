namespace SurveyBackend.Domain.Departments;

public class Department
{
    public Guid Id { get; }
    public string Name { get; }
    public string ExternalIdentifier { get; }

    public Department(Guid id, string name, string externalIdentifier)
    {
        Id = id;
        Name = name;
        ExternalIdentifier = externalIdentifier;
    }
}
