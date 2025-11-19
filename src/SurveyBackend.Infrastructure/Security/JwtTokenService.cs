using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<JwtTokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_settings.AccessTokenMinutes);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("username", user.Username),
            new("departmentId", user.DepartmentId.ToString()),
            new("permissions", "[]")
        };

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        var accessToken = _tokenHandler.WriteToken(jwt);
        var refreshToken = GenerateSecureToken();
        var refreshExpiresAt = now.AddDays(_settings.RefreshTokenDays);
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_settings.AccessTokenMinutes).TotalSeconds;

        var result = new JwtTokenResult(accessToken, expiresAt, refreshToken, refreshExpiresAt, expiresInSeconds);
        return Task.FromResult(result);
    }

    private static string GenerateSecureToken()
    {
        Span<byte> buffer = stackalloc byte[64];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }
}
