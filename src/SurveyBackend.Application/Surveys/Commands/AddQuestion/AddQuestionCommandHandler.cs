using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand, Guid>
{
    private readonly ISurveyRepository _surveyRepository;

    public AddQuestionCommandHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<Guid> HandleAsync(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        var questionDto = request.Question ?? throw new ArgumentNullException(nameof(request.Question));
        var question = survey.AddQuestion(Guid.NewGuid(), questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);

        if (questionDto.Options is not null)
        {
            foreach (var option in questionDto.Options)
            {
                question.AddOption(Guid.NewGuid(), option.Text, option.Order, option.Value);
            }
        }

        await _surveyRepository.UpdateAsync(survey, cancellationToken);
        return question.Id;
    }
}
