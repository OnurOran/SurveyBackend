using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IAppMediator _mediator;

    public PermissionsController(IAppMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PermissionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetPermissionsQuery, IReadOnlyCollection<PermissionDto>>(new GetPermissionsQuery(), cancellationToken);
        return Ok(response);
    }
}
