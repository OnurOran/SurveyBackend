namespace SurveyBackend.Domain.Users;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public Guid DepartmentId { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsLocalUser { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();

    private User()
    {
    }

    private User(Guid id, string username, string email, Guid departmentId, bool isLocalUser, string? passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Username = username;
        Email = email;
        DepartmentId = departmentId;
        IsLocalUser = isLocalUser;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public static User Create(Guid id, string username, string email, Guid departmentId, DateTimeOffset createdAt)
    {
        return new User(id, username, email, departmentId, false, null, createdAt);
    }

    public static User CreateLocal(Guid id, string username, string email, Guid departmentId, string passwordHash, DateTimeOffset createdAt)
    {
        return new User(id, username, email, departmentId, true, passwordHash, createdAt);
    }

    public void UpdateContactInformation(string email, Guid departmentId, DateTimeOffset updatedAt)
    {
        Email = email;
        DepartmentId = departmentId;
        UpdatedAt = updatedAt;
    }

    public RefreshToken IssueRefreshToken(string token, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), Id, token, expiresAt, createdAt);
        RefreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void UpdateLocalPassword(string passwordHash, DateTimeOffset updatedAt)
    {
        PasswordHash = passwordHash;
        IsLocalUser = true;
        UpdatedAt = updatedAt;
    }
}
