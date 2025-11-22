using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Roles;

namespace SurveyBackend.Domain.Users;

public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid DepartmentId { get; private set; }

    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public Department? Department { get; private set; }

    private UserRole()
    {
    }

    public UserRole(Guid userId, Guid roleId, Guid departmentId)
    {
        UserId = userId;
        RoleId = roleId;
        DepartmentId = departmentId;
    }
}
