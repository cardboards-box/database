namespace CardboardBox.Database.Sqlite;

/// <summary>
/// Provides a <see cref="ISqlService"/> capable of connecting to a SQLite Database
/// </summary>
public class SqliteService : SqlService<SqliteConnection>
{
	private readonly ISqlConfig _config;
	private readonly IConnectionInitProvider _init;
	private static bool _initRun = true;

	/// <summary>
	/// Provides a <see cref="ISqlService"/> capable of connecting to a SQLite Database
	/// </summary>
	/// <param name="config">The configuration for this connection</param>
	/// <param name="init">A collection of things to do when the connection is made</param>
	public SqliteService(
		ISqlConfig<SqliteConnection> config,
		IConnectionInitProvider init) : base(config) 
	{ 
		_config = config;
		_init = init;
	}

	/// <summary>
	/// Creates a new <see cref="SqliteConnection"/> and opens it
	/// </summary>
	/// <returns>The created ADO.Net SQL connection</returns>
	public override async Task<IDbConnection> CreateConnection()
	{
		var conString = _config.ConnectionString;
		var con = new SqliteConnection(conString);

		if (con.State != ConnectionState.Open)
			await con.OpenAsync();

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
