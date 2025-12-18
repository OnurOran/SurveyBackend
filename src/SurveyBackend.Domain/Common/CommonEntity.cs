namespace SurveyBackend.Domain.Common;

/// <summary>
/// Base entity class with audit trail and soft delete support
/// </summary>
public abstract class CommonEntity
{
    /// <summary>
    /// ID of the employee who created this record
    /// </summary>
    public string? CreateEmployeeId { get; protected set; }

    /// <summary>
    /// Date and time when this record was created (Turkey timezone assumed)
    /// </summary>
    public DateTime CreateDate { get; protected set; }

    /// <summary>
    /// ID of the employee who last updated this record
    /// </summary>
    public string? UpdateEmployeeId { get; protected set; }

    /// <summary>
    /// Date and time when this record was last updated (Turkey timezone assumed)
    /// </summary>
    public DateTime? UpdateDate { get; protected set; }

    /// <summary>
    /// Indicates whether this record is deleted (soft delete)
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Indicates whether this record is active
    /// </summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public byte[]? RowVersion { get; protected set; }

    /// <summary>
    /// Marks this entity as deleted (soft delete)
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
    }

    /// <summary>
    /// Restores this entity from soft delete
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
    }

    /// <summary>
    /// Deactivates this entity
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Activates this entity
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Sets audit fields for entity creation
    /// </summary>
    public void SetCreatedAudit(string? employeeId, DateTime timestamp)
    {
        CreateEmployeeId = employeeId;
        CreateDate = timestamp;
    }

    /// <summary>
    /// Sets audit fields for entity update
    /// </summary>
    public void SetUpdatedAudit(string? employeeId, DateTime timestamp)
    {
        UpdateEmployeeId = employeeId;
        UpdateDate = timestamp;
    }
}
