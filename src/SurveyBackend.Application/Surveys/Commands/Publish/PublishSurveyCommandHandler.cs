using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed class PublishSurveyCommandHandler : ICommandHandler<PublishSurveyCommand, bool>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(PublishSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        survey.Publish(request.StartDate, request.EndDate);

        await _unitOfWork.ExecuteAsync(ct => _surveyRepository.UpdateAsync(survey, ct), cancellationToken);
        return true;
    }
}
