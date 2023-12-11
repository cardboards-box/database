namespace CardboardBox.Database.Postgres.Standard.Attributes;

/// <summary>
/// Represents a table type in Postgres
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TypeAttribute : Attribute
{
    /// <summary>
    /// The name of the table type
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Represents a table type in Postgres
    /// </summary>
    /// <param name="name">The name of the table type</param>
    public TypeAttribute(string name)
    {
        Name = name;
    }
}