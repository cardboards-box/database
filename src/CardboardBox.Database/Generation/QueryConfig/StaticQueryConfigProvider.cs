namespace CardboardBox.Database.Generation;

/// <summary>
/// Provides a query configuration that was setup at runtime
/// </summary>
/// <remarks>
/// Provides a query configuration that was setup at runtime
/// </remarks>
/// <param name="config">The query configuration</param>
public class StaticQueryConfigProvider(QueryConfig config) : IQueryConfigProvider
{
	private readonly QueryConfig _config = config;

    /// <summary>
    /// Provides the default query configuration
    /// </summary>
    /// <returns>The query configuration</returns>
    public QueryConfig GetQueryConfig() => _config;
}
