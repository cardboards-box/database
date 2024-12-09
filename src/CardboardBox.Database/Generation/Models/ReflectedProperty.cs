namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a class property that has been resolved via reflection
/// </summary>
/// <param name="property">The target property</param>
/// <param name="column">The optional <see cref="ColumnAttribute"/> that was found on the property</param>
public class ReflectedProperty(PropertyInfo property, ColumnAttribute? column)
{
    /// <summary>
    /// The target property
    /// </summary>
    public PropertyInfo Property { get; } = property;

    /// <summary>
    /// The optional <see cref="ColumnAttribute"/> that was found on the property
    /// </summary>
    public ColumnAttribute? Column { get; } = column;

    /// <summary>
    /// The name to use when referencing the column in a query
    /// </summary>
    public string Name => Column?.Name ?? Property.Name;

	/// <summary>
	/// Whether or not to ignore this property
	/// </summary>
	public bool Ignore => Column?.Ignore ?? false;
}
