namespace CardboardBox.Database;

/// <summary>
/// Marks a property as a column to use within the table
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
	/// <summary>
	/// The name of the column.
	/// If not specified, the property name will be used
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// The value to use whenever this column is referenced in a query.
	/// This is mostly for default columns to specific values **ALWAYS**, 
	/// like ensuring that audit fields use CURRENT_TIMESTAMP for their initial and subsequent values.
	/// </summary>
	public string? OverrideValue { get; set; }

	/// <summary>
	/// Marks the column as being the primary key for the table.
	/// This can only be specified once per table and can cause errors if multiple are specified.
	/// </summary>
	public bool PrimaryKey { get; set; } = false;

	/// <summary>
	/// Whether or not to exclude this column when doing UPDATE queries.
	/// </summary>
	public bool ExcludeUpdates { get; set; } = false;

	/// <summary>
	/// Whether or not to exclude this column when doing INSERT queries
	/// </summary>
	public bool ExcludeInserts { get; set; } = false;

	/// <summary>
	/// Whether or not this column should be unique.
	/// If multiple columns are specified, they will be assumed to be part of the same unique column set.
	/// </summary>
	public bool Unique { get; set; } = false;

	/// <summary>
	/// Whether or not to ignore this column when creating queries.
	/// </summary>
	public bool Ignore { get; set; } = false;

	/// <summary>
	/// Marks a property as a column to use within the table
	/// </summary>
	public ColumnAttribute() { }

	/// <summary>
	/// Marks a property as a column to use within the table
	/// </summary>
	/// <param name="name">The name of the column.</param>
	public ColumnAttribute(string name)
	{
		Name = name;
	}
}
