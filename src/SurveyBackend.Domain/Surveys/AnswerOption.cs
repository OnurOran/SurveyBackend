using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class AnswerOption : CommonEntity
{
    public int Id { get; private set; }
    public int AnswerId { get; private set; }
    public int QuestionOptionId { get; private set; }

    public Answer Answer { get; private set; } = null!;
    public QuestionOption QuestionOption { get; private set; } = null!;

    private AnswerOption()
    {
    }

    public AnswerOption(int answerId, int questionOptionId)
    {
        AnswerId = answerId;
        QuestionOptionId = questionOptionId;
    }
}
