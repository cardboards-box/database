namespace CardboardBox.Database.Postgres;

/// <summary>
/// Represents a service used for creating and managing the <see cref="NpgsqlDataSource"/>
/// </summary>
public interface IDataSourceService
{
	/// <summary>
	/// Gets or creates the current instance of the <see cref="NpgsqlDataSource"/>
	/// </summary>
	/// <returns></returns>
	Task<NpgsqlDataSource> DataSource();
}

/// <summary>
/// A service used for creating and managing the <see cref="NpgsqlDataSource"/>
/// </summary>
/// <param name="config">The SQL connection configuration</param>
/// <param name="init">A collection of things to do when the connection is first initialized</param>
public class DataSourceService(
    ISqlConfig<NpgsqlConnection> config,
    IConnectionInitProvider init) : IDataSourceService
{
	private readonly ISqlConfig _config = config;
	private readonly IConnectionInitProvider _init = init;

	private NpgsqlDataSource? _dataSource;

    /// <summary>
    /// Gets or creates the current instance of the <see cref="NpgsqlDataSource"/>
    /// </summary>
    /// <returns></returns>
    public async Task<NpgsqlDataSource> DataSource()
	{
		if (_dataSource != null) return _dataSource;

		var builder = new NpgsqlDataSourceBuilder(_config.ConnectionString);

		foreach (var action in _init.Builder)
			await action(builder);

		return _dataSource = builder.Build();
	}
}
