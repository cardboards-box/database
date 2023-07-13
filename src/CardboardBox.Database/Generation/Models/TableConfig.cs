namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a minified version of <see cref="ReflectedType"/>
/// </summary>
/// <param name="Name">The name of the table</param>
/// <param name="Prefixes">Any prefixes to prepend to the table name</param>
public record class TableConfig(
	string Name,
	string[] Prefixes);
