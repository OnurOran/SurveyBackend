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
    [HttpGet("status/{slug}")]
    public async Task<ActionResult<ParticipationStatusResult>> CheckStatus(string slug, CancellationToken cancellationToken)
    {
        var surveyNumber = ParseSurveyNumberFromSlug(slug);
        if (surveyNumber == 0)
        {
            return BadRequest("Geçersiz anket URL formatı.");
        }

        Guid? externalId = null;

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            if (Request.Cookies.TryGetValue(ParticipantCookieName, out var cookieValue) && Guid.TryParse(cookieValue, out var cookieId))
            {
                externalId = cookieId;
            }
        }

        var query = new CheckParticipationStatusQuery(surveyNumber, externalId);
        var result = await _mediator.SendAsync<CheckParticipationStatusQuery, ParticipationStatusResult>(query, cancellationToken);
        return Ok(result);
    }

    private static int ParseSurveyNumberFromSlug(string slug)
    {
        // Format: {slug}-{number}, e.g., "musteri-memnuniyet-anketi-42"
        // Extract the number from the end
        var lastHyphenIndex = slug.LastIndexOf('-');
        if (lastHyphenIndex == -1 || lastHyphenIndex == slug.Length - 1)
        {
            return 0;
        }

        var numberPart = slug.Substring(lastHyphenIndex + 1);
        return int.TryParse(numberPart, out var number) ? number : 0;
    }

    [AllowAnonymous]
    [HttpPost("start")]
    public async Task<ActionResult<int>> Start([FromBody] StartParticipationRequest request, CancellationToken cancellationToken)
    {
        var surveyNumber = ParseSurveyNumberFromSlug(request.Slug);
        if (surveyNumber == 0)
        {
            return BadRequest("Geçersiz anket URL formatı.");
        }

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
                    Expires = DateTime.UtcNow.AddYears(1)
                });
            }

            externalId = cookieId;
        }

        var command = new StartParticipationCommand(surveyNumber, externalId)
        {
            IpAddress = ipAddress
        };
        var participationId = await _mediator.SendAsync<StartParticipationCommand, int>(command, cancellationToken);
        return Ok(participationId);
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/answers")]
    public async Task<IActionResult> SubmitAnswer(int id, [FromBody] SubmitAnswerRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitAnswerCommand(id, request.QuestionId, request.TextValue, request.OptionIds, request.Attachment);
        await _mediator.SendAsync<SubmitAnswerCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        var command = new CompleteParticipationCommand(id);
        await _mediator.SendAsync<CompleteParticipationCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
