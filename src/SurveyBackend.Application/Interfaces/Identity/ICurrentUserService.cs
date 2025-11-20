using System;

namespace SurveyBackend.Application.Interfaces.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? DepartmentId { get; }
    bool IsAuthenticated { get; }
    bool IsLocalAdmin { get; }
    bool HasPermission(string permission);
}
