namespace CardboardBox.Database.Postgres.Standard.Attributes;

/// <summary>
/// Represents a table type in Postgres
/// </summary>
/// <param name="name">The name of the table type</param>
[AttributeUsage(AttributeTargets.Class)]
public class TypeAttribute(string name) : Attribute
{
    /// <summary>
    /// The name of the table type
    /// </summary>
    public string Name { get; set; } = name;
}