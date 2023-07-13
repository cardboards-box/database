namespace CardboardBox.Database.Generation;

/// <summary>
/// Provides a query configuration that was setup at runtime
/// </summary>
public class StaticQueryConfigProvider : IQueryConfigProvider
{
	private readonly QueryConfig _config;

	/// <summary>
	/// Provides a query configuration that was setup at runtime
	/// </summary>
	/// <param name="config">The query configuration</param>
	public StaticQueryConfigProvider(QueryConfig config)
	{
		_config = config;
	}

	/// <summary>
	/// Provides the default query configuration
	/// </summary>
	/// <returns>The query configuration</returns>
	public QueryConfig GetQueryConfig() => _config;
}
