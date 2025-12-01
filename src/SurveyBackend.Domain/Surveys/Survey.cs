using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Survey
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IntroText { get; private set; }
    public string? ConsentText { get; private set; }
    public string? OutroText { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public Guid DepartmentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public AccessType AccessType { get; private set; }
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }

    public Attachment? Attachment { get; private set; }
    public ICollection<Question> Questions { get; private set; } = new List<Question>();

    private Survey()
    {
    }

    private Survey(Guid id, string title, string? description, string? introText, string? consentText, string? outroText, string createdBy, Guid departmentId, AccessType accessType, DateTimeOffset createdAt, DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        Id = id;
        Title = title;
        Description = description;
        IntroText = introText;
        ConsentText = consentText;
        OutroText = outroText;
        CreatedBy = createdBy;
        DepartmentId = departmentId;
        AccessType = accessType;
        CreatedAt = createdAt;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = false;
    }

    public static Survey Create(Guid id, string title, string? description, string? introText, string? consentText, string? outroText, string createdBy, Guid departmentId, AccessType accessType, DateTimeOffset createdAt, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Survey title cannot be empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy cannot be empty.", nameof(createdBy));
        }

        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException("DepartmentId cannot be empty.", nameof(departmentId));
        }

        if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        return new Survey(id, title.Trim(), description?.Trim(), introText?.Trim(), consentText?.Trim(), outroText?.Trim(), createdBy.Trim(), departmentId, accessType, createdAt, startDate, endDate);
    }

    public void Publish(DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
    {
        var effectiveStart = startDate ?? StartDate;
        var effectiveEnd = endDate ?? EndDate;

        if (effectiveStart.HasValue && effectiveEnd.HasValue && effectiveEnd <= effectiveStart)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        StartDate = effectiveStart;
        EndDate = effectiveEnd;
        IsActive = true;
    }

    public Question AddQuestion(Guid id, string text, QuestionType type, int order, bool isRequired, string? description = null)
    {
        var question = Question.Create(id, Id, text, order, type, isRequired, description);
        Questions.Add(question);
        return question;
    }
}
