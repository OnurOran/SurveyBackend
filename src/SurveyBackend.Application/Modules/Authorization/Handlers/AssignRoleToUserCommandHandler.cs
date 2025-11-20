using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class AssignRoleToUserCommandHandler : ICommandHandler<AssignRoleToUserCommand, bool>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public AssignRoleToUserCommandHandler(
        IAuthorizationService authorizationService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _authorizationService = authorizationService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<bool> HandleAsync(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");

        await _authorizationService.EnsureRoleManagementAsync(user.DepartmentId, cancellationToken);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
                   ?? throw new InvalidOperationException("Rol bulunamadı.");

        await _userRoleRepository.AssignRoleAsync(user.Id, role.Id, cancellationToken);
        return true;
    }
}
