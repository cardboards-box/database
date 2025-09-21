namespace CardboardBox.Database;

/// <summary>
/// Indicates that a table should include reference expansions for all tables.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class AuditAttribute(string type, string id) : Attribute
{
    /// <summary>
    /// The name of the column that stores the type of the record
    /// </summary>
    public string TypeColumnName { get; } = type;

    /// <summary>
    /// The name of the column that stores the ID of the record
    /// </summary>
    public string IdColumnName { get; } = id;
}
