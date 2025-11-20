namespace SurveyBackend.Domain.Roles;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public ICollection<RolePermission> Permissions { get; private set; } = new List<RolePermission>();

    private Role()
    {
    }

    public Role(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
