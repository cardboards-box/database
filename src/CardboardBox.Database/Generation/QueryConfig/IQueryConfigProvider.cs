namespace CardboardBox.Database.Generation;

/// <summary>
/// Provides the default query configuration
/// </summary>
public interface IQueryConfigProvider
{
	/// <summary>
	/// Provides the default query configuration
	/// </summary>
	/// <returns>The query configuration</returns>
	QueryConfig GetQueryConfig();
}
