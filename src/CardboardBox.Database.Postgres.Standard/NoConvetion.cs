using Dapper.FluentMap.Conventions;

namespace CardboardBox.Database.Postgres.Standard;

/// <summary>
/// A naming convention that doesn't change the property names
/// </summary>
public class NoChangeConvention : Convention
{
	/// <inheritdoc />
	public NoChangeConvention() { }
}
