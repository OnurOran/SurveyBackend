namespace SurveyBackend.Domain.Surveys;

public class AnswerOption
{
    public Guid Id { get; private set; }
    public Guid AnswerId { get; private set; }
    public Guid QuestionOptionId { get; private set; }

    public Answer Answer { get; private set; } = null!;
    public QuestionOption QuestionOption { get; private set; } = null!;

    private AnswerOption()
    {
    }

    public AnswerOption(Guid id, Guid answerId, Guid questionOptionId)
    {
        Id = id;
        AnswerId = answerId;
        QuestionOptionId = questionOptionId;
    }
}
