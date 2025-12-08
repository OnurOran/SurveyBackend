using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Participations.Commands.CompleteParticipation;
using SurveyBackend.Application.Participations.Commands.StartParticipation;
using SurveyBackend.Application.Participations.Commands.SubmitAnswer;
using SurveyBackend.Application.Participations.DTOs;
using SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipationsController : ControllerBase
{
    private const string ParticipantCookieName = "X-Survey-Participant";
    private readonly IAppMediator _mediator;

    public ParticipationsController(IAppMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("status/{surveyId:guid}")]
    public async Task<ActionResult<ParticipationStatusResult>> CheckStatus(Guid surveyId, CancellationToken cancellationToken)
    {
        Guid? externalId = null;

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            if (Request.Cookies.TryGetValue(ParticipantCookieName, out var cookieValue) && Guid.TryParse(cookieValue, out var cookieId))
            {
                externalId = cookieId;
            }
        }

        var query = new CheckParticipationStatusQuery(surveyId, externalId);
        var result = await _mediator.SendAsync<CheckParticipationStatusQuery, ParticipationStatusResult>(query, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("start")]
    public async Task<ActionResult<Guid>> Start([FromBody] StartParticipationRequest request, CancellationToken cancellationToken)
    {
        Guid? externalId = null;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            if (!Request.Cookies.TryGetValue(ParticipantCookieName, out var cookieValue) || !Guid.TryParse(cookieValue, out var cookieId))
            {
                cookieId = Guid.NewGuid();
                Response.Cookies.Append(ParticipantCookieName, cookieId.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }

            externalId = cookieId;
        }

        var command = new StartParticipationCommand(request.SurveyId, externalId)
        {
            IpAddress = ipAddress
        };
        var participationId = await _mediator.SendAsync<StartParticipationCommand, Guid>(command, cancellationToken);
        return Ok(participationId);
    }

    [AllowAnonymous]
    [HttpPost("{id:guid}/answers")]
    public async Task<IActionResult> SubmitAnswer(Guid id, [FromBody] SubmitAnswerRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitAnswerCommand(id, request.QuestionId, request.TextValue, request.OptionIds, request.Attachment);
        await _mediator.SendAsync<SubmitAnswerCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var command = new CompleteParticipationCommand(id);
        await _mediator.SendAsync<CompleteParticipationCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
