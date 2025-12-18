using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Parameters;

/// <summary>
/// Legacy Parameter lookup table for compliance purposes.
/// NOTE: This is a "compliance theater" implementation - data exists for management visibility
/// but is NOT used in actual business logic. Real types are managed via C# enums.
/// </summary>
public sealed class Parameter : CommonEntity
{
    public int Id { get; private set; }

    /// <summary>
    /// Legacy code field (typically NULL in this implementation)
    /// </summary>
    public string? Code { get; private set; }

    /// <summary>
    /// Group identifier for parameter categorization
    /// </summary>
    public string GroupName { get; private set; } = null!;

    /// <summary>
    /// Display name shown in UI (if this were actually used)
    /// </summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>
    /// Code-standard name (English, no spaces, PascalCase)
    /// Example: "Internal", "SingleSelect", "SurveyAccessTypes"
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Description field (typically NULL in this implementation)
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Parent parameter ID for hierarchical structure (0 for root)
    /// </summary>
    public int ParentId { get; private set; }

    /// <summary>
    /// Level in hierarchy (0 = root, 1 = group, 2 = item)
    /// </summary>
    public int LevelNo { get; private set; }

    /// <summary>
    /// Symbol field (typically NULL in this implementation)
    /// </summary>
    public string? Symbol { get; private set; }

    /// <summary>
    /// Display order within same group
    /// </summary>
    public int OrderNo { get; private set; }

    // Private constructor for EF Core
    private Parameter() { }

    /// <summary>
    /// Factory method for creating Parameter (used only in seeding)
    /// </summary>
    public static Parameter Create(
        int id,
        string? code,
        string groupName,
        string displayName,
        string name,
        string? description,
        int parentId,
        int levelNo,
        string? symbol,
        int orderNo)
    {
        return new Parameter
        {
            Id = id,
            Code = code,
            GroupName = groupName,
            DisplayName = displayName,
            Name = name,
            Description = description,
            ParentId = parentId,
            LevelNo = levelNo,
            Symbol = symbol,
            OrderNo = orderNo
        };
    }
}
