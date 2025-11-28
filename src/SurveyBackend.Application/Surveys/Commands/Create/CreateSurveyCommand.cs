using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.Create;

public sealed record CreateSurveyCommand(
    string Title,
    string? Description,
    AccessType AccessType,
    List<CreateQuestionDto>? Questions,
    AttachmentUploadDto? Attachment = null) : ICommand<Guid>;
