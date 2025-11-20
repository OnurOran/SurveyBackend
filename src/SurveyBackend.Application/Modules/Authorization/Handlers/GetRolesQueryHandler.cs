using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class GetRolesQueryHandler : ICommandHandler<GetRolesQuery, IReadOnlyCollection<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IReadOnlyCollection<RoleDto>> HandleAsync(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(role =>
                new RoleDto(
                    role.Id,
                    role.Name,
                    role.Description,
                    role.Permissions.Select(rp => rp.Permission.Name).ToList()))
            .ToList();
    }
}
