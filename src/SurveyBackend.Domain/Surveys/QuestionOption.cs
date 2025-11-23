namespace SurveyBackend.Domain.Surveys;

public class QuestionOption
{
    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public int Order { get; private set; }
    public int? Value { get; private set; }

    public Question Question { get; private set; } = null!;
    public ICollection<DependentQuestion> DependentQuestions { get; private set; } = new List<DependentQuestion>();

    private QuestionOption()
    {
    }

    private QuestionOption(Guid id, Guid questionId, string text, int order, int? value)
    {
        Id = id;
        QuestionId = questionId;
        Text = text;
        Order = order;
        Value = value;
    }

    internal static QuestionOption Create(Guid id, Guid questionId, string text, int order, int? value)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Question option text cannot be empty.", nameof(text));
        }

        return new QuestionOption(id, questionId, text.Trim(), order, value);
    }

    public DependentQuestion AddDependentQuestion(Guid id, Guid childQuestionId)
    {
        var dependentQuestion = new DependentQuestion(id, Id, childQuestionId);
        DependentQuestions.Add(dependentQuestion);
        return dependentQuestion;
    }
}
