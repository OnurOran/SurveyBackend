namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record CreateOptionDto(string Text, int Order, int? Value, AttachmentUploadDto? Attachment = null);
