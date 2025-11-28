using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Attachment
{
    public Guid Id { get; private set; }
    public AttachmentOwnerType OwnerType { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid? SurveyId { get; private set; }
    public Guid? QuestionId { get; private set; }
    public Guid? QuestionOptionId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public Survey? Survey { get; private set; }
    public Question? Question { get; private set; }
    public QuestionOption? QuestionOption { get; private set; }

    private Attachment()
    {
    }

    private Attachment(
        Guid id,
        AttachmentOwnerType ownerType,
        Guid departmentId,
        Guid? surveyId,
        Guid? questionId,
        Guid? questionOptionId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset createdAt)
    {
        Id = id;
        OwnerType = ownerType;
        DepartmentId = departmentId;
        SurveyId = surveyId;
        QuestionId = questionId;
        QuestionOptionId = questionOptionId;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
        CreatedAt = createdAt;
    }

    public static Attachment CreateForSurvey(
        Guid id,
        Guid surveyId,
        Guid departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset createdAt)
    {
        return new Attachment(id, AttachmentOwnerType.Survey, departmentId, surveyId, null, null, fileName, contentType, sizeBytes, storagePath, createdAt);
    }

    public static Attachment CreateForQuestion(
        Guid id,
        Guid questionId,
        Guid departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset createdAt)
    {
        return new Attachment(id, AttachmentOwnerType.Question, departmentId, null, questionId, null, fileName, contentType, sizeBytes, storagePath, createdAt);
    }

    public static Attachment CreateForOption(
        Guid id,
        Guid questionOptionId,
        Guid departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset createdAt)
    {
        return new Attachment(id, AttachmentOwnerType.Option, departmentId, null, null, questionOptionId, fileName, contentType, sizeBytes, storagePath, createdAt);
    }
}
