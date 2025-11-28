using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AnswerAttachmentAccessInfo(
    Guid AttachmentId,
    Guid SurveyId,
    Guid DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    Guid ParticipationId,
    Guid AnswerId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string StoragePath);
