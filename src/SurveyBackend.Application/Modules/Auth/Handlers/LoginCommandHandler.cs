using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthTokensDto>
{
    private readonly ILdapService _ldapService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        ILdapService ldapService,
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService)
    {
        _ldapService = ldapService;
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthTokensDto> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var ldapUser = await _ldapService.AuthenticateAsync(request.Username, request.Password, cancellationToken);
        if (ldapUser is null)
        {
            throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
        }

        var department = await _departmentRepository.GetByExternalIdentifierAsync(ldapUser.DepartmentName, cancellationToken)
                         ?? throw new InvalidOperationException("Departman eşlemesi bulunamadı.");

        var existingUser = await _userRepository.GetByUsernameAsync(ldapUser.Username, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        User user;

        if (existingUser is null)
        {
            user = User.Create(ldapUser.ExternalId == Guid.Empty ? Guid.NewGuid() : ldapUser.ExternalId, ldapUser.Username, ldapUser.Email, department.Id, now);
            await _userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            existingUser.UpdateContactInformation(ldapUser.Email, department.Id, now);
            await _userRepository.UpdateAsync(existingUser, cancellationToken);
            user = existingUser;
        }

        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);
        var refreshToken = user.IssueRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt, now);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        var refreshTokenDto = AuthMapper.ToRefreshTokenDto(refreshToken);
        var authTokens = AuthMapper.ToAuthTokensDto(new TokenResult(tokenResult.AccessToken, refreshTokenDto.Token, tokenResult.AccessTokenExpiresInSeconds));
        return authTokens;
    }
}
