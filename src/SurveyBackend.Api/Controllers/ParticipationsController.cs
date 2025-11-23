using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Participations.Commands.CompleteParticipation;
using SurveyBackend.Application.Participations.Commands.StartParticipation;
using SurveyBackend.Application.Participations.Commands.SubmitAnswer;

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
    [HttpPost("start")]
    public async Task<ActionResult<Guid>> Start([FromBody] StartParticipationCommand command, CancellationToken cancellationToken)
    {
        Guid? externalId = null;

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

        var commandWithExternal = command with { ExternalId = externalId };
        var participationId = await _mediator.SendAsync<StartParticipationCommand, Guid>(commandWithExternal, cancellationToken);
        return Ok(participationId);
    }

    [AllowAnonymous]
    [HttpPost("{id:guid}/answers")]
    public async Task<IActionResult> SubmitAnswer(Guid id, [FromBody] SubmitAnswerCommand command, CancellationToken cancellationToken)
    {
        var enrichedCommand = command with { ParticipationId = id };
        await _mediator.SendAsync<SubmitAnswerCommand, bool>(enrichedCommand, cancellationToken);
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
