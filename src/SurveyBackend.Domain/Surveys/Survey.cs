using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Survey : CommonEntity
{
    public int Id { get; private set; }
    public string Slug { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IntroText { get; private set; }
    public string? ConsentText { get; private set; }
    public string? OutroText { get; private set; }
    public int DepartmentId { get; private set; }
    public bool IsPublished { get; private set; }
    public AccessType AccessType { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public Attachment? Attachment { get; private set; }
    public ICollection<Question> Questions { get; private set; } = new List<Question>();

    private Survey()
    {
    }

    private Survey(string title, string? description, string? introText, string? consentText, string? outroText, int departmentId, AccessType accessType, DateTime? startDate, DateTime? endDate)
    {
        Slug = GenerateSlug(title);
        Title = title;
        Description = description;
        IntroText = introText;
        ConsentText = consentText;
        OutroText = outroText;
        DepartmentId = departmentId;
        AccessType = accessType;
        StartDate = startDate;
        EndDate = endDate;
        IsPublished = false;
    }

    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "anket";
        }

        // Turkish character replacements
        var slug = title.ToLowerInvariant()
            .Replace('ı', 'i')
            .Replace('ğ', 'g')
            .Replace('ü', 'u')
            .Replace('ş', 's')
            .Replace('ö', 'o')
            .Replace('ç', 'c')
            .Replace('İ', 'i')
            .Replace('Ğ', 'g')
            .Replace('Ü', 'u')
            .Replace('Ş', 's')
            .Replace('Ö', 'o')
            .Replace('Ç', 'c');

        // Replace non-alphanumeric with hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9]+", "-");

        // Remove leading/trailing hyphens and multiple consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        // Limit length to 100 characters
        if (slug.Length > 100)
        {
            slug = slug.Substring(0, 100).TrimEnd('-');
        }

        return string.IsNullOrWhiteSpace(slug) ? "anket" : slug;
    }

    public static Survey Create(string title, string? description, string? introText, string? consentText, string? outroText, int departmentId, AccessType accessType, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Survey title cannot be empty.", nameof(title));
        }

        if (departmentId <= 0)
        {
            throw new ArgumentException("DepartmentId must be greater than 0.", nameof(departmentId));
        }

        if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        return new Survey(title.Trim(), description?.Trim(), introText?.Trim(), consentText?.Trim(), outroText?.Trim(), departmentId, accessType, startDate, endDate);
    }

    public void Publish(DateTime? startDate = null, DateTime? endDate = null)
    {
        var effectiveStart = startDate ?? StartDate;
        var effectiveEnd = endDate ?? EndDate;

        if (effectiveStart.HasValue && effectiveEnd.HasValue && effectiveEnd <= effectiveStart)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        StartDate = effectiveStart;
        EndDate = effectiveEnd;
        IsPublished = true;
    }

    public Question AddQuestion(string text, QuestionType type, int order, bool isRequired, string? description = null)
    {
        var question = Question.Create(Id, text, order, type, isRequired, description);
        Questions.Add(question);
        return question;
    }

    public void Update(string title, string? description, string? introText, string? consentText, string? outroText, AccessType accessType)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Survey title cannot be empty.", nameof(title));
        }

        Title = title.Trim();
        Slug = GenerateSlug(title);
        Description = description?.Trim();
        IntroText = introText?.Trim();
        ConsentText = consentText?.Trim();
        OutroText = outroText?.Trim();
        AccessType = accessType;
    }
}
