namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed record PublishSurveyCommand(Guid SurveyId, DateTimeOffset? StartDate, DateTimeOffset? EndDate) : ICommand<bool>;
