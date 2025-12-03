using SurveyBackend.Domain.Enums;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Api.Contracts;

public sealed class UpdateSurveyRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IntroText { get; set; }
    public string? ConsentText { get; set; }
    public string? OutroText { get; set; }
    public AccessType AccessType { get; set; }
    public List<CreateQuestionDto>? Questions { get; set; }
    public AttachmentUploadDto? Attachment { get; set; }
}
