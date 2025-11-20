namespace SurveyBackend.Application.Abstractions.Messaging;

public interface IDepartmentScopedCommand
{
    Guid DepartmentId { get; }
}
