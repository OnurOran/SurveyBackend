using System.Linq;

namespace SurveyBackend.Domain.Surveys;

public class Answer
{
    public Guid Id { get; private set; }
    public Guid ParticipationId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string? TextValue { get; private set; }

    public Participation Participation { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public ICollection<AnswerOption> SelectedOptions { get; private set; } = new List<AnswerOption>();

    private Answer()
    {
    }

    internal Answer(Guid id, Guid participationId, Guid questionId, string? textValue)
    {
        Id = id;
        ParticipationId = participationId;
        QuestionId = questionId;
        TextValue = textValue?.Trim();
    }

    public AnswerOption AddSelectedOption(Guid id, Guid questionOptionId)
    {
        var selection = new AnswerOption(id, Id, questionOptionId);
        SelectedOptions.Add(selection);
        return selection;
    }

    public void Update(string? textValue)
    {
        TextValue = textValue?.Trim();
    }

    public void ReplaceSelectedOptions(IEnumerable<Guid>? optionIds)
    {
        var newOptionIds = (optionIds ?? Enumerable.Empty<Guid>()).Distinct().ToList();

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
            SelectedOptions.Add(new AnswerOption(Guid.NewGuid(), Id, optionId));
        }
    }
}
