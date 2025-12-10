using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed class GetSurveyReportQueryHandler : ICommandHandler<GetSurveyReportQuery, SurveyReportDto?>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSurveyReportQueryHandler(
        ISurveyRepository surveyRepository,
        IParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SurveyReportDto?> HandleAsync(GetSurveyReportQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
        if (survey is null)
        {
            return null;
        }

        // Check access permission
        if (!HasManagementAccess(survey))
        {
            return null;
        }

        // Get all participations for this survey
        var participations = await _participationRepository.GetBySurveyIdAsync(request.SurveyId, cancellationToken);

        var totalParticipations = participations.Count;
        var completedParticipations = participations.Count(p => p.CompletedAt.HasValue);
        var completionRate = totalParticipations > 0 ? (double)completedParticipations / totalParticipations * 100 : 0;

        // Build participant list (for internal surveys)
        var participantList = survey.AccessType == AccessType.Internal
            ? participations.Select(p => new ParticipantSummaryDto
            {
                ParticipationId = p.Id,
                ParticipantName = p.Participant?.LdapUsername ?? "Unknown",
                IsCompleted = p.CompletedAt.HasValue,
                StartedAt = p.StartedAt.UtcDateTime
            }).ToList()
            : new List<ParticipantSummaryDto>();

        // Collect child question IDs to filter them from main list
        var childQuestionIds = survey.Questions
            .SelectMany(q => q.Options)
            .SelectMany(o => o.DependentQuestions)
            .Select(dq => dq.ChildQuestionId)
            .ToHashSet();

        // Process each question
        var questionReports = survey.Questions
            .Where(q => !childQuestionIds.Contains(q.Id))
            .OrderBy(q => q.Order)
            .Select(q => BuildQuestionReport(q, participations, survey.AccessType == AccessType.Internal))
            .ToList();

        return new SurveyReportDto
        {
            SurveyId = survey.Id,
            Title = survey.Title,
            Description = survey.Description ?? string.Empty,
            IntroText = survey.IntroText,
            OutroText = survey.OutroText,
            AccessType = survey.AccessType.ToString(),
            StartDate = survey.StartDate?.UtcDateTime,
            EndDate = survey.EndDate?.UtcDateTime,
            IsActive = survey.IsActive,
            TotalParticipations = totalParticipations,
            CompletedParticipations = completedParticipations,
            CompletionRate = Math.Round(completionRate, 2),
            Participants = participantList,
            Questions = questionReports,
            Attachment = survey.Attachment != null ? new AttachmentDto(
                survey.Attachment.Id,
                survey.Attachment.FileName,
                survey.Attachment.ContentType,
                survey.Attachment.SizeBytes
            ) : null
        };
    }

    private QuestionReportDto BuildQuestionReport(Question question, IReadOnlyList<Participation> participations, bool isInternalSurvey)
    {
        var answers = participations
            .SelectMany(p => p.Answers)
            .Where(a => a.QuestionId == question.Id)
            .ToList();

        var totalResponses = answers.Count;
        var totalParticipations = participations.Count;
        var responseRate = totalParticipations > 0 ? (double)totalResponses / totalParticipations * 100 : 0;

        QuestionReportDto report = question.Type switch
        {
            QuestionType.SingleSelect => BuildSingleSelectReport(question, answers, totalResponses, responseRate),
            QuestionType.MultiSelect => BuildMultiSelectReport(question, answers, totalResponses, responseRate),
            QuestionType.OpenText => BuildOpenTextReport(question, answers, participations, totalResponses, responseRate, isInternalSurvey),
            QuestionType.FileUpload => BuildFileUploadReport(question, answers, participations, totalResponses, responseRate, isInternalSurvey),
            QuestionType.Conditional => BuildConditionalReport(question, participations, totalResponses, responseRate, isInternalSurvey),
            _ => new QuestionReportDto
            {
                QuestionId = question.Id,
                Text = question.Text,
                Type = question.Type.ToString(),
                Order = question.Order,
                IsRequired = question.IsRequired,
                TotalResponses = totalResponses,
                ResponseRate = Math.Round(responseRate, 2),
                Attachment = question.Attachment != null ? new AttachmentDto(
                    question.Attachment.Id,
                    question.Attachment.FileName,
                    question.Attachment.ContentType,
                    question.Attachment.SizeBytes
                ) : null
            }
        };

        return report;
    }

    private QuestionReportDto BuildSingleSelectReport(Question question, List<Answer> answers, int totalResponses, double responseRate)
    {
        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));
                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults
        };
    }

    private QuestionReportDto BuildMultiSelectReport(Question question, List<Answer> answers, int totalResponses, double responseRate)
    {
        // For multi-select, each option can be selected independently
        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));
                // Percentage is based on total answers, not total selections
                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults
        };
    }

    private QuestionReportDto BuildOpenTextReport(Question question, List<Answer> answers, IReadOnlyList<Participation> participations, int totalResponses, double responseRate, bool isInternalSurvey)
    {
        var textResponses = answers
            .Where(a => !string.IsNullOrWhiteSpace(a.TextValue))
            .Select(a =>
            {
                var participation = participations.FirstOrDefault(p => p.Id == a.ParticipationId);
                var participantName = isInternalSurvey && participation?.Participant != null
                    ? participation.Participant.LdapUsername
                    : null;

                return new TextResponseDto
                {
                    ParticipationId = a.ParticipationId,
                    ParticipantName = participantName,
                    TextValue = a.TextValue ?? string.Empty,
                    SubmittedAt = participation?.CompletedAt?.UtcDateTime ?? participation?.StartedAt.UtcDateTime ?? DateTime.UtcNow
                };
            })
            .OrderByDescending(r => r.SubmittedAt)
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            TextResponses = textResponses
        };
    }

    private QuestionReportDto BuildFileUploadReport(Question question, List<Answer> answers, IReadOnlyList<Participation> participations, int totalResponses, double responseRate, bool isInternalSurvey)
    {
        var fileResponses = answers
            .Where(a => a.Attachment != null)
            .Select(a =>
            {
                var participation = participations.FirstOrDefault(p => p.Id == a.ParticipationId);
                var participantName = isInternalSurvey && participation?.Participant != null
                    ? participation.Participant.LdapUsername
                    : null;

                return new FileResponseDto
                {
                    AnswerId = a.Id,
                    AttachmentId = a.Attachment!.Id,
                    ParticipationId = a.ParticipationId,
                    ParticipantName = participantName,
                    FileName = a.Attachment.FileName,
                    ContentType = a.Attachment.ContentType,
                    SizeBytes = a.Attachment.SizeBytes,
                    SubmittedAt = participation?.CompletedAt?.UtcDateTime ?? participation?.StartedAt.UtcDateTime ?? DateTime.UtcNow
                };
            })
            .OrderByDescending(r => r.SubmittedAt)
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            FileResponses = fileResponses
        };
    }

    private QuestionReportDto BuildConditionalReport(Question question, IReadOnlyList<Participation> participations, int totalResponses, double responseRate, bool isInternalSurvey)
    {
        // For conditional questions, group by parent option
        var conditionalResults = question.Options
            .Where(o => o.DependentQuestions.Any())
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                // Count participants who selected this option
                var participantsWhoSelectedOption = participations
                    .Where(p => p.Answers.Any(a =>
                        a.QuestionId == question.Id &&
                        a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id)))
                    .ToList();

                var childQuestionReports = option.DependentQuestions
                    .OrderBy(dq => dq.ChildQuestion.Order)
                    .Select(dq => BuildQuestionReport(dq.ChildQuestion, participantsWhoSelectedOption, isInternalSurvey))
                    .ToList();

                return new ConditionalBranchResultDto
                {
                    ParentOptionId = option.Id,
                    ParentOptionText = option.Text,
                    ParticipantCount = participantsWhoSelectedOption.Count,
                    ChildQuestions = childQuestionReports
                };
            })
            .ToList();

        // Also calculate option results for the parent question itself
        var answers = participations
            .SelectMany(p => p.Answers)
            .Where(a => a.QuestionId == question.Id)
            .ToList();

        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));
                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults,
            ConditionalResults = conditionalResults
        };
    }

    private bool HasManagementAccess(Survey survey)
    {
        if (_currentUserService.IsSuperAdmin || _currentUserService.HasPermission("ManageUsers"))
        {
            return true;
        }

        if (_currentUserService.HasPermission("ManageDepartment") &&
            _currentUserService.DepartmentId.HasValue &&
            _currentUserService.DepartmentId.Value == survey.DepartmentId)
        {
            return true;
        }

        return false;
    }
}
