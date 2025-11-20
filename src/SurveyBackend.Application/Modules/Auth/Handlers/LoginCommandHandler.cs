using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthTokensDto>
{
    private readonly ILdapService _ldapService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public LoginCommandHandler(
        ILdapService ldapService,
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _ldapService = ldapService;
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<AuthTokensDto> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var localUser = await _userRepository.GetLocalByUsernameAsync(request.Username, cancellationToken);
        User user;

        if (localUser is not null && localUser.IsLocalUser)
        {
            if (string.IsNullOrWhiteSpace(localUser.PasswordHash) || !_passwordHasher.Verify(localUser.PasswordHash, request.Password))
            {
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
            }

            user = localUser;
        }
        else
        {
            var ldapUser = await _ldapService.AuthenticateAsync(request.Username, request.Password, cancellationToken);
            if (ldapUser is null)
            {
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
            }

            var department = await _departmentRepository.GetByExternalIdentifierAsync(ldapUser.DepartmentName, cancellationToken)
                             ?? throw new InvalidOperationException("Departman eşlemesi bulunamadı.");

            var existingUser = await _userRepository.GetByUsernameAsync(ldapUser.Username, cancellationToken);

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
        }

        var permissions = await ResolvePermissionsAsync(user, cancellationToken);
        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, permissions, cancellationToken);
        var refreshToken = user.IssueRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt, now);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        var refreshTokenDto = AuthMapper.ToRefreshTokenDto(refreshToken);
        var authTokens = AuthMapper.ToAuthTokensDto(new TokenResult(tokenResult.AccessToken, refreshTokenDto.Token, tokenResult.AccessTokenExpiresInSeconds));
        return authTokens;
    }

    private async Task<IReadOnlyCollection<string>> ResolvePermissionsAsync(User user, CancellationToken cancellationToken)
    {
        if (user.IsLocalUser)
        {
            var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
            return allPermissions.Select(p => p.Name).ToList();
        }

        return await _userRoleRepository.GetPermissionsForUserAsync(user.Id, cancellationToken);
    }
}
