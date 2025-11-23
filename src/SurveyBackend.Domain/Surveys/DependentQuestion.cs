namespace SurveyBackend.Domain.Surveys;

public class DependentQuestion
{
    public Guid Id { get; private set; }
    public Guid ParentQuestionOptionId { get; private set; }
    public Guid ChildQuestionId { get; private set; }

    public QuestionOption ParentOption { get; private set; } = null!;
    public Question ChildQuestion { get; private set; } = null!;

    private DependentQuestion()
    {
    }

    public DependentQuestion(Guid id, Guid parentQuestionOptionId, Guid childQuestionId)
    {
        Id = id;
        ParentQuestionOptionId = parentQuestionOptionId;
        ChildQuestionId = childQuestionId;
    }
}
