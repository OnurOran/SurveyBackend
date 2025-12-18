using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed record SubmitAnswerCommand(int ParticipationId, int QuestionId, string? TextValue, List<int>? OptionIds, AttachmentUploadDto? Attachment = null) : ICommand<bool>;
