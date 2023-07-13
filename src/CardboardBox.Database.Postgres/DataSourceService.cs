namespace CardboardBox.Database.Postgres;

/// <summary>
/// Represents a service used for creating and mangaing the <see cref="NpgsqlDataSource"/>
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
/// A service used for creating and mangaing the <see cref="NpgsqlDataSource"/>
/// </summary>
public class DataSourceService : IDataSourceService
{
	private readonly ISqlConfig _config;
	private readonly IConnectionInitProvider _init;

	private NpgsqlDataSource? _dataSource;

	/// <summary>
	/// A service used for creating and mangaing the <see cref="NpgsqlDataSource"/>
	/// </summary>
	/// <param name="config">The SQL connection configuration</param>
	/// <param name="init">A collection of things to do when the connection is first initialized</param>
	public DataSourceService(
		ISqlConfig<NpgsqlConnection> config,
		IConnectionInitProvider init)
	{
		_config = config;
		_init = init;
	}

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
