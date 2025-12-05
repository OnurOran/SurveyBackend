namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyReportDto
{
    public Guid SurveyId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AccessType { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsActive { get; init; }
    public int TotalParticipations { get; init; }
    public int CompletedParticipations { get; init; }
    public double CompletionRate { get; init; }
    public IReadOnlyList<ParticipantSummaryDto> Participants { get; init; } = Array.Empty<ParticipantSummaryDto>();
    public IReadOnlyList<QuestionReportDto> Questions { get; init; } = Array.Empty<QuestionReportDto>();
}

public sealed record ParticipantSummaryDto
{
    public Guid ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime StartedAt { get; init; }
}

public sealed record QuestionReportDto
{
    public Guid QuestionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int Order { get; init; }
    public bool IsRequired { get; init; }
    public int TotalResponses { get; init; }
    public double ResponseRate { get; init; }

    // For SingleSelect and MultiSelect
    public IReadOnlyList<OptionResultDto>? OptionResults { get; init; }

    // For OpenText
    public IReadOnlyList<TextResponseDto>? TextResponses { get; init; }

    // For FileUpload
    public IReadOnlyList<FileResponseDto>? FileResponses { get; init; }

    // For Conditional questions
    public IReadOnlyList<ConditionalBranchResultDto>? ConditionalResults { get; init; }
}

public sealed record OptionResultDto
{
    public Guid OptionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
    public int SelectionCount { get; init; }
    public double Percentage { get; init; }
}

public sealed record TextResponseDto
{
    public Guid ParticipationId { get; init; }
    public string? ParticipantName { get; init; } // Only for internal surveys
    public string TextValue { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}

public sealed record FileResponseDto
{
    public Guid AnswerId { get; init; }
    public Guid ParticipationId { get; init; }
    public string? ParticipantName { get; init; } // Only for internal surveys
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public DateTime SubmittedAt { get; init; }
}

public sealed record ConditionalBranchResultDto
{
    public Guid ParentOptionId { get; init; }
    public string ParentOptionText { get; init; } = string.Empty;
    public int ParticipantCount { get; init; }
    public IReadOnlyList<QuestionReportDto> ChildQuestions { get; init; } = Array.Empty<QuestionReportDto>();
}

/// <summary>
/// Individual participant's complete responses (for searching by name)
/// </summary>
public sealed record ParticipantResponseDto
{
    public Guid ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public IReadOnlyList<ParticipantAnswerDto> Answers { get; init; } = Array.Empty<ParticipantAnswerDto>();
}

public sealed record ParticipantAnswerDto
{
    public Guid QuestionId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string? TextValue { get; init; }
    public IReadOnlyList<string> SelectedOptions { get; init; } = Array.Empty<string>();
    public string? FileName { get; init; }
    public Guid? AnswerId { get; init; } // For file downloads
}
