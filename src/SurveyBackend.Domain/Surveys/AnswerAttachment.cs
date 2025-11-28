namespace SurveyBackend.Domain.Surveys;

public class AnswerAttachment
{
    public Guid Id { get; private set; }
    public Guid AnswerId { get; private set; }
    public Guid SurveyId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public Answer Answer { get; private set; } = null!;

    private AnswerAttachment()
    {
    }

    private AnswerAttachment(Guid id, Guid answerId, Guid surveyId, Guid departmentId, string fileName, string contentType, long sizeBytes, string storagePath, DateTimeOffset createdAt)
    {
        Id = id;
        AnswerId = answerId;
        SurveyId = surveyId;
        DepartmentId = departmentId;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
        CreatedAt = createdAt;
    }

    public static AnswerAttachment Create(Guid id, Guid answerId, Guid surveyId, Guid departmentId, string fileName, string contentType, long sizeBytes, string storagePath, DateTimeOffset createdAt)
    {
        return new AnswerAttachment(id, answerId, surveyId, departmentId, fileName, contentType, sizeBytes, storagePath, createdAt);
    }
}
