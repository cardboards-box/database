namespace CardboardBox.Database;

/// <summary>
/// Marks a class as representing a database table
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableAttribute : Attribute
{
	/// <summary>
	/// The name of the table in the database.
	/// If not specified, it will fallback on the class name
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Any prefixes, in order, to apply to the table name.
	/// If not specified, no prefixes will be applied.
	/// </summary>
	public string[] Prefixes { get; set; } = Array.Empty<string>();

	/// <summary>
	/// Marks a class as representing a database table
	/// </summary>
	public TableAttribute() { }

	/// <summary>
	/// Marks a class as representing a database table
	/// </summary>
	/// <param name="name">The name of the table in the database</param>
	/// <param name="prefixes">Any prefixes, in order, to apply to the table name.</param>
	public TableAttribute(string name, params string[] prefixes)
	{
		Name = name;
		Prefixes = prefixes;
	}
}
