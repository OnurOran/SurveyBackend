namespace SurveyBackend.Application.Modules.Auth.Models;

public sealed record JwtTokenResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    int AccessTokenExpiresInSeconds);
