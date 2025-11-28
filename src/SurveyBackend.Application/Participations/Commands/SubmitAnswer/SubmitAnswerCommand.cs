using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed record SubmitAnswerCommand(Guid ParticipationId, Guid QuestionId, string? TextValue, List<Guid>? OptionIds, AttachmentUploadDto? Attachment = null) : ICommand<bool>;
