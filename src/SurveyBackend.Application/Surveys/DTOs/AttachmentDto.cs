namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AttachmentDto(Guid Id, string FileName, string ContentType, long SizeBytes);
