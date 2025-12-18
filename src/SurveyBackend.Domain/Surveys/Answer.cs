using System.Linq;
using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class Answer : CommonEntity
{
    public int Id { get; private set; }
    public int ParticipationId { get; private set; }
    public int QuestionId { get; private set; }
    public string? TextValue { get; private set; }

    public Participation Participation { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public AnswerAttachment? Attachment { get; private set; }
    public ICollection<AnswerOption> SelectedOptions { get; private set; } = new List<AnswerOption>();

    private Answer()
    {
    }

    internal Answer(int participationId, int questionId, string? textValue)
    {
        ParticipationId = participationId;
        QuestionId = questionId;
        TextValue = textValue?.Trim();
    }

    public AnswerOption AddSelectedOption(int questionOptionId)
    {
        var selection = new AnswerOption(Id, questionOptionId);
        SelectedOptions.Add(selection);
        return selection;
    }

    public void Update(string? textValue)
    {
        TextValue = textValue?.Trim();
    }

    public void ReplaceSelectedOptions(IEnumerable<int>? optionIds)
    {
        var newOptionIds = (optionIds ?? Enumerable.Empty<int>()).Distinct().ToList();

        // Remove options that are no longer selected
        var optionsToRemove = SelectedOptions
            .Where(ao => !newOptionIds.Contains(ao.QuestionOptionId))
            .ToList();

        foreach (var option in optionsToRemove)
        {
            SelectedOptions.Remove(option);
        }

        // Add new options that weren't previously selected
        var existingOptionIds = SelectedOptions.Select(ao => ao.QuestionOptionId).ToHashSet();
        var optionIdsToAdd = newOptionIds.Where(oid => !existingOptionIds.Contains(oid));

        foreach (var optionId in optionIdsToAdd)
        {
            SelectedOptions.Add(new AnswerOption(Id, optionId));
        }
    }
}
