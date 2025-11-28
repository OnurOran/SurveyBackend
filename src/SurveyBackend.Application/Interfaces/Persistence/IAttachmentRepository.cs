using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IAttachmentRepository
{
    Task<Attachment?> GetByOwnerAsync(AttachmentOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken);
    Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Attachment attachment, CancellationToken cancellationToken);
    Task RemoveAsync(Attachment attachment, CancellationToken cancellationToken);
    Task<AttachmentAccessInfo?> GetAccessInfoAsync(Guid attachmentId, CancellationToken cancellationToken);
}
