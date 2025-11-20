namespace SurveyBackend.Domain.Roles;

public class Permission
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Permission()
    {
    }

    public Permission(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
