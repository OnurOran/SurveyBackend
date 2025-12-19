namespace SurveyBackend.Domain.Common;

/// <summary>
/// Base entity class with audit trail and soft delete support
/// </summary>
public abstract class CommonEntity
{
    /// <summary>
    /// Indicates whether this record is active
    /// </summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Indicates whether this record is deleted (soft delete)
    /// </summary>
    public bool IsDelete { get; protected set; }

    /// <summary>
    /// Date and time when this record was created (Turkey timezone assumed)
    /// </summary>
    public DateTime CreateDate { get; protected set; }

    /// <summary>
    /// ID of the employee (User ID) who created this record
    /// </summary>
    public int? CreateEmployeeId { get; protected set; }

    /// <summary>
    /// Date and time when this record was last updated (Turkey timezone assumed)
    /// </summary>
    public DateTime? UpdateDate { get; protected set; }

    /// <summary>
    /// ID of the employee (User ID) who last updated this record
    /// </summary>
    public int? UpdateEmployeeId { get; protected set; }

    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public byte[]? RowVersion { get; protected set; }

    /// <summary>
    /// Marks this entity as deleted (soft delete)
    /// </summary>
    public void Delete()
    {
        IsDelete = true;
    }

    /// <summary>
    /// Restores this entity from soft delete
    /// </summary>
    public void Restore()
    {
        IsDelete = false;
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
    public void SetCreatedAudit(int? employeeId, DateTime timestamp)
    {
        CreateEmployeeId = employeeId;
        CreateDate = timestamp;
    }

    /// <summary>
    /// Sets audit fields for entity update
    /// </summary>
    public void SetUpdatedAudit(int? employeeId, DateTime timestamp)
    {
        UpdateEmployeeId = employeeId;
        UpdateDate = timestamp;
    }
}
