using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand, Guid>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAuthorizationService _authorizationService;

    public AddQuestionCommandHandler(ISurveyRepository surveyRepository, IAuthorizationService authorizationService)
    {
        _surveyRepository = surveyRepository;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> HandleAsync(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

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
