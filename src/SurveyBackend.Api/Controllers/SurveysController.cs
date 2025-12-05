using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Surveys.Commands.AddQuestion;
using SurveyBackend.Application.Surveys.Commands.Create;
using SurveyBackend.Application.Surveys.Commands.Update;
using SurveyBackend.Application.Surveys.Commands.Publish;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Queries.GetSurvey;
using SurveyBackend.Application.Surveys.Queries.GetDepartmentSurveys;
using SurveyBackend.Application.Surveys.Queries.GetSurveys;
using SurveyBackend.Application.Surveys.Queries.GetSurveyReport;
using SurveyBackend.Api.Contracts;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public SurveysController(IAppMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsers)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SurveyListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>(new GetSurveysQuery(), cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("department")]
    public async Task<ActionResult<IReadOnlyCollection<SurveyListItemDto>>> GetMyDepartmentSurveys(CancellationToken cancellationToken)
    {
        var departmentId = _currentUserService.DepartmentId;
        if (!departmentId.HasValue)
        {
            return Forbid();
        }

        var query = new GetDepartmentSurveysQuery(departmentId.Value);
        var response = await _mediator.SendAsync<GetDepartmentSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>(query, cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("department/{departmentId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<SurveyListItemDto>>> GetByDepartment(Guid departmentId, CancellationToken cancellationToken)
    {
        var query = new GetDepartmentSurveysQuery(departmentId);
        var response = await _mediator.SendAsync<GetDepartmentSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>(query, cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurveyCommand command, CancellationToken cancellationToken)
    {
        var surveyId = await _mediator.SendAsync<CreateSurveyCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetSurvey), new { id = surveyId }, null);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSurveyRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSurveyCommand(
            id,
            request.Title,
            request.Description,
            request.IntroText,
            request.ConsentText,
            request.OutroText,
            request.AccessType,
            request.Questions,
            request.Attachment);

        await _mediator.SendAsync<UpdateSurveyCommand, bool>(command, cancellationToken);
        return NoContent();
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

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("{id:guid}/report")]
    public async Task<ActionResult<SurveyReportDto>> GetReport(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSurveyReportQuery(id);
        var response = await _mediator.SendAsync<GetSurveyReportQuery, SurveyReportDto?>(query, cancellationToken);
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("{id:guid}/report/participant")]
    public async Task<ActionResult<ParticipantResponseDto>> GetParticipantResponse(Guid id, [FromQuery] string participantName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(participantName))
        {
            return BadRequest("Participant name is required");
        }

        var query = new GetParticipantResponseQuery(id, participantName);
        var response = await _mediator.SendAsync<GetParticipantResponseQuery, ParticipantResponseDto?>(query, cancellationToken);
        if (response is null)
        {
            return NotFound("Participant not found or survey is not internal");
        }

        return Ok(response);
    }
}
