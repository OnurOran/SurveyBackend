using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Surveys.Commands.AddQuestion;
using SurveyBackend.Application.Surveys.Commands.Create;
using SurveyBackend.Application.Surveys.Commands.Publish;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Queries.GetSurvey;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly IAppMediator _mediator;

    public SurveysController(IAppMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurveyCommand command, CancellationToken cancellationToken)
    {
        var surveyId = await _mediator.SendAsync<CreateSurveyCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetSurvey), new { id = surveyId }, null);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SurveyDetailDto>> GetSurvey(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetSurveyQuery, SurveyDetailDto?>(new GetSurveyQuery(id), cancellationToken);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPost("{id:guid}/questions")]
    public async Task<IActionResult> AddQuestion(Guid id, [FromBody] CreateQuestionDto questionDto, CancellationToken cancellationToken)
    {
        if (questionDto is null)
        {
            return BadRequest("Geçersiz soru isteği.");
        }

        var command = new AddQuestionCommand(id, questionDto);
        await _mediator.SendAsync<AddQuestionCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetSurvey), new { id }, null);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, [FromBody] PublishSurveyRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Geçersiz yayınlama isteği.");
        }

        var command = new PublishSurveyCommand(id, request.StartDate, request.EndDate);
        await _mediator.SendAsync<PublishSurveyCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
