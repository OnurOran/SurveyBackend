using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Queries.GetSurvey;

public sealed class GetSurveyQueryHandler : ICommandHandler<GetSurveyQuery, SurveyDetailDto?>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetSurveyQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<SurveyDetailDto?> HandleAsync(GetSurveyQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (survey is null)
        {
            return null;
        }

        if (!IsAvailable(survey))
        {
            return null;
        }

        var questions = survey.Questions
            .OrderBy(q => q.Order)
            .Select(q => new SurveyQuestionDetailDto(
                q.Id,
                q.Text,
                q.Description,
                q.Type,
                q.Order,
                q.IsRequired,
                q.Options
                    .OrderBy(o => o.Order)
                    .Select(o => new SurveyOptionDetailDto(o.Id, o.Text, o.Order, o.Value, MapAttachment(o.Attachment)))
                    .ToList(),
                MapAttachment(q.Attachment),
                q.GetAllowedAttachmentContentTypes()))
            .ToList();

        return new SurveyDetailDto(
            survey.Id,
            survey.Title,
            survey.Description,
            survey.IntroText,
            survey.ConsentText,
            survey.OutroText,
            survey.AccessType,
            survey.StartDate,
            survey.EndDate,
            questions,
            MapAttachment(survey.Attachment));
    }

    private static bool IsAvailable(Survey survey)
    {
        var now = DateTimeOffset.UtcNow;

        if (!survey.IsActive)
        {
            return false;
        }

        if (survey.StartDate.HasValue && survey.StartDate.Value > now)
        {
            return false;
        }

        if (survey.EndDate.HasValue && survey.EndDate.Value <= now)
        {
            return false;
        }

        return true;
    }

    private static AttachmentDto? MapAttachment(Domain.Surveys.Attachment? attachment)
    {
        if (attachment is null)
        {
            return null;
        }

        return new AttachmentDto(attachment.Id, attachment.FileName, attachment.ContentType, attachment.SizeBytes);
    }
}
