using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Application.Modules.Auth.Models;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthTokensDto> HandleAsync(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken is null || !existingToken.IsActive)
        {
            throw new UnauthorizedAccessException("Geçersiz refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");

        var now = DateTimeOffset.UtcNow;
        existingToken.Revoke(now);
        await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);

        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);
        var newRefreshToken = user.IssueRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt, now);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var refreshTokenDto = AuthMapper.ToRefreshTokenDto(newRefreshToken);
        return AuthMapper.ToAuthTokensDto(new TokenResult(tokenResult.AccessToken, refreshTokenDto.Token, tokenResult.AccessTokenExpiresInSeconds));
    }
}
