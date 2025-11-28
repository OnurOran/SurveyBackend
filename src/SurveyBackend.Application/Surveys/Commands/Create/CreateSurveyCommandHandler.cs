using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Commands.Create;

public sealed class CreateSurveyCommandHandler : ICommandHandler<CreateSurveyCommand, Guid>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public CreateSurveyCommandHandler(
        ISurveyRepository surveyRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> HandleAsync(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
        }

        var creator = _currentUserService.UserId.Value.ToString();
        var now = DateTimeOffset.UtcNow;
        var departmentId = _currentUserService.DepartmentId
            ?? throw new UnauthorizedAccessException("Kullanıcının departman bilgisi bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(departmentId, cancellationToken);

        var survey = Survey.Create(Guid.NewGuid(), request.Title, request.Description, creator, departmentId, request.AccessType, now);

        if (request.Questions is not null)
        {
            foreach (var questionDto in request.Questions)
            {
                var question = survey.AddQuestion(Guid.NewGuid(), questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
                if (questionDto.Options is null)
                {
                    continue;
                }

                foreach (var optionDto in questionDto.Options)
                {
                    question.AddOption(Guid.NewGuid(), optionDto.Text, optionDto.Order, optionDto.Value);
                }
            }
        }

        await _surveyRepository.AddAsync(survey, cancellationToken);

        return survey.Id;
    }
}
