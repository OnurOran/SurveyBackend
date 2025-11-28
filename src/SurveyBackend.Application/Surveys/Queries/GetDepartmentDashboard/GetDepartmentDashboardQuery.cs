using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentDashboard;

public sealed record GetDepartmentDashboardQuery(Guid DepartmentId) : ICommand<DepartmentDashboardDto>, IDepartmentScopedCommand;
