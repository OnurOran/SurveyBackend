namespace SurveyBackend.Domain.Users;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public User? User { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken()
    {
    }

    private RefreshToken(Guid id, Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public static RefreshToken Create(Guid id, Guid userId, string token, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        return new RefreshToken(id, userId, token, expiresAt, createdAt);
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        RevokedAt = revokedAt;
    }
}
