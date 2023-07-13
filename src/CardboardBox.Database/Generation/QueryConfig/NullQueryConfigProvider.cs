namespace CardboardBox.Database.Generation;

/// <summary>
/// Default query configuration provider
/// </summary>
public class NullQueryConfigProvider : IQueryConfigProvider
{
	/// <summary>
	/// Provides the default query configuration
	/// </summary>
	/// <returns>The query configuration</returns>
	public QueryConfig GetQueryConfig() => new();
}
