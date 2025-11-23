using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Question
{
    public Guid Id { get; private set; }
    public Guid SurveyId { get; private set; }
    public string Text { get; private set; } = null!;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public QuestionType Type { get; private set; }
    public bool IsRequired { get; private set; }

    public Survey Survey { get; private set; } = null!;
    public ICollection<QuestionOption> Options { get; private set; } = new List<QuestionOption>();

    private Question()
    {
    }

    private Question(Guid id, Guid surveyId, string text, int order, QuestionType type, bool isRequired, string? description)
    {
        Id = id;
        SurveyId = surveyId;
        Text = text;
        Description = description;
        Order = order;
        Type = type;
        IsRequired = isRequired;
    }

    internal static Question Create(Guid id, Guid surveyId, string text, int order, QuestionType type, bool isRequired, string? description)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Question text cannot be empty.", nameof(text));
        }

        return new Question(id, surveyId, text.Trim(), order, type, isRequired, description?.Trim());
    }

    public QuestionOption AddOption(Guid id, string text, int order, int? value = null)
    {
        var option = QuestionOption.Create(id, Id, text, order, value);
        Options.Add(option);
        return option;
    }
}
