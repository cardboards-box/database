namespace CardboardBox.Database.Generation;

using PropMap = Dictionary<string, ReflectedProperty>;

/// <summary>
/// Represents a class that has been resolved via reflection
/// </summary>
/// <param name="type">The underlying class type</param>
/// <param name="table">The optional <see cref="TableAttribute"/> that was found on the class</param>
/// <param name="properties">A collection of all of the properties found on the type</param>
public class ReflectedType(Type type, TableAttribute? table, PropMap properties)
{
    /// <summary>
    /// The underlying class type
    /// </summary>
    public Type Type { get; } = type;

    /// <summary>
    /// The optional <see cref="TableAttribute"/> that was found on the class
    /// </summary>
    public TableAttribute? Table { get; } = table;

    /// <summary>
    /// A collection of all of the properties found on the type
    /// </summary>
    public PropMap Properties { get; } = properties;

    /// <summary>
    /// The name (and any prefixes) of this type.
    /// </summary>
    public TableConfig Name => new(Table?.Name ?? Type.Name, Table?.Prefixes ?? []);
}
