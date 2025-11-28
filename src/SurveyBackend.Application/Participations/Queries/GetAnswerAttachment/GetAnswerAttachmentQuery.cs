using SurveyBackend.Application.Surveys.Queries.GetAttachment;

namespace SurveyBackend.Application.Participations.Queries.GetAnswerAttachment;

public sealed record GetAnswerAttachmentQuery(Guid AttachmentId) : ICommand<AttachmentDownloadResult>;
