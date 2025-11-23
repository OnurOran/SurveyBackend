using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record CreateQuestionDto(string Text, QuestionType Type, int Order, bool IsRequired, List<CreateOptionDto>? Options);
