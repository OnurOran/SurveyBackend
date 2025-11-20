using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class GetPermissionsQueryHandler : ICommandHandler<GetPermissionsQuery, IReadOnlyCollection<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionsQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IReadOnlyCollection<PermissionDto>> HandleAsync(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        return permissions
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .ToList();
    }
}
