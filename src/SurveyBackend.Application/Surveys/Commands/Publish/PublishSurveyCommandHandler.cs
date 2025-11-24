using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed class PublishSurveyCommandHandler : ICommandHandler<PublishSurveyCommand, bool>
{
    private readonly ISurveyRepository _surveyRepository;

    public PublishSurveyCommandHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<bool> HandleAsync(PublishSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        survey.Publish(request.StartDate, request.EndDate);

        await _surveyRepository.UpdateAsync(survey, cancellationToken);
        return true;
    }
}
