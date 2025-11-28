using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Participations.DTOs;

public sealed record SubmitAnswerRequest(Guid QuestionId, string? TextValue, List<Guid>? OptionIds, AttachmentUploadDto? Attachment = null);
