using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AttachmentAccessInfo(
    Guid AttachmentId,
    Guid SurveyId,
    Guid DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string FileName,
    string ContentType,
    long SizeBytes,
    string StoragePath);
