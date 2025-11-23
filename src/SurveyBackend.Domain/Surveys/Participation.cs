using System.Linq;

namespace SurveyBackend.Domain.Surveys;

public class Participation
{
    public Guid Id { get; private set; }
    public Guid SurveyId { get; private set; }
    public Guid ParticipantId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string? IpAddress { get; private set; }

    public Participant Participant { get; private set; } = null!;
    public ICollection<Answer> Answers { get; private set; } = new List<Answer>();

    private Participation()
    {
    }

    private Participation(Guid id, Guid surveyId, Guid participantId, DateTimeOffset startedAt, string? ipAddress)
    {
        Id = id;
        SurveyId = surveyId;
        ParticipantId = participantId;
        StartedAt = startedAt;
        IpAddress = ipAddress;
    }

    public static Participation Start(Guid id, Guid surveyId, Guid participantId, DateTimeOffset startedAt, string? ipAddress = null)
    {
        return new Participation(id, surveyId, participantId, startedAt, ipAddress);
    }

    public void Complete(DateTimeOffset completedAt)
    {
        CompletedAt = completedAt;
    }

    public Answer AddOrUpdateAnswer(Guid id, Guid questionId, string? textValue, IEnumerable<Guid>? optionIds = null)
    {
        var answer = Answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (answer is null)
        {
            answer = new Answer(id, Id, questionId, textValue);
            Answers.Add(answer);
        }
        else
        {
            answer.Update(textValue);
        }

        answer.ReplaceSelectedOptions(optionIds);
        return answer;
    }
}
