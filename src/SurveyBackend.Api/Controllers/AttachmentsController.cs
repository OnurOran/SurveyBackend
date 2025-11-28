using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Application.Surveys.Queries.GetAttachment;
using SurveyBackend.Application.Participations.Queries.GetAnswerAttachment;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IFileStorage _fileStorage;

    public AttachmentsController(IAppMediator mediator, IFileStorage fileStorage)
    {
        _mediator = mediator;
        _fileStorage = fileStorage;
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendAsync<GetAttachmentQuery, AttachmentDownloadResult>(new GetAttachmentQuery(id), cancellationToken);
        var stream = await _fileStorage.OpenReadAsync(result.StoragePath, cancellationToken);
        return File(stream, result.ContentType, result.FileName);
    }

    [AllowAnonymous]
    [HttpGet("answers/{id:guid}")]
    public async Task<IActionResult> DownloadAnswerAttachment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendAsync<GetAnswerAttachmentQuery, AttachmentDownloadResult>(new GetAnswerAttachmentQuery(id), cancellationToken);
        var stream = await _fileStorage.OpenReadAsync(result.StoragePath, cancellationToken);
        return File(stream, result.ContentType, result.FileName);
    }
}
