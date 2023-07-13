using System.Data;

namespace CardboardBox.Database.Postgres;

/// <summary>
/// Provides a <see cref="ISqlService"/> capable of connecting to a Postgre Database
/// </summary>
public class NpgsqlService : SqlService<NpgsqlConnection>
{
	private readonly IDataSourceService _datasource;
	private readonly IConnectionInitProvider _init;
	private static bool _initRun = true;

	/// <summary>
	/// Provides a <see cref="ISqlService"/> capable of connecting to a Postgre Database
	/// </summary>
	/// <param name="config">The configuration for this connection</param>
	/// <param name="datasource">The data source management service</param>
	/// <param name="init">A collection of things to do when the connection is made</param>
	public NpgsqlService(
		ISqlConfig<NpgsqlConnection> config,
		IDataSourceService datasource,
		IConnectionInitProvider init) : base(config)
	{
		_datasource = datasource;
		_init = init;
	}

	/// <summary>
	/// Creates a new <see cref="NpgsqlConnection"/> and opens it
	/// </summary>
	/// <returns>The opened ADO.Net <see cref="IDbConnection"/>.</returns>
	public override async Task<IDbConnection> CreateConnection()
	{
		var bob = await _datasource.DataSource();

		var con = await bob.OpenConnectionAsync();

		if (_initRun)
		{
			_initRun = false;
			foreach (var action in _init.InitialRun)
				await action(con);
		}

		foreach (var action in _init.Connect)
			await action(con);

		return con;
	}
}
