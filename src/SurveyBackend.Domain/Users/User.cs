namespace SurveyBackend.Domain.Users;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public Guid DepartmentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User()
    {
    }

    private User(Guid id, string username, string email, Guid departmentId, DateTimeOffset createdAt)
    {
        Id = id;
        Username = username;
        Email = email;
        DepartmentId = departmentId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public static User Create(Guid id, string username, string email, Guid departmentId, DateTimeOffset createdAt)
    {
        return new User(id, username, email, departmentId, createdAt);
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
}
