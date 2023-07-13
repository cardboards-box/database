namespace CardboardBox.Database;

using Generation;
using Sqlite;

using Con = SqliteConnection;
using Service = Sqlite.SqliteService;
using Config = ISqlConfig<SqliteConnection>;
using Init = Action<Sqlite.IConnectionInitBuilder>;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> that configures the SQL Service and SQL generation systems
/// </summary>
public static class DiExtensions
{
	/// <summary>
	/// Registers all of the internal services with the configuration builder for SQLite
	/// </summary>
	/// <param name="bob">The configuration builder (get it? bob the builder...? Ha ha)</param>
	/// <param name="init">The initalization configuration action</param>
	public static void RegisterInternalServices(ISqlConfigurationBuilder bob, Init? init)
	{
		var provider = new ConnectionInitBuilder();
		init?.Invoke(provider);

		bob.AddInternalServices(c =>
		{
			c.AddSingleton<IConnectionInitProvider>(provider);
		});

		bob.ConfigureGeneration(c =>
		{
			c.WithQueryGen<SqliteQueryGenerationService>()
			 .WithConfig(new QueryConfig
			 {
				 EscapeStart = "\"",
				 EscapeEnd = "\"",
				 ParameterCharacter = "@"
			 });
		});
	}

	/// <summary>
	/// Registers SQLite with the configuration builder
	/// </summary>
	/// <typeparam name="T">The type of <see cref="ISqlConfig"/> to use</typeparam>
	/// <param name="builder">The configuration builder to add SQLite to</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddSQLite<T>(this ISqlConfigurationBuilder builder, Init? init = null)
		where T : class, Config
	{
		builder.AddSqlEngine<Con, Service, T>();
		RegisterInternalServices(builder, init);
		return builder;
	}

	/// <summary>
	/// Registers SQLite with the configuration builder
	/// </summary>
	/// <param name="builder">The configuration builder to add SQLite to</param>
	/// <param name="config">The configuration to use for SQLite connections</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddSQLite(this ISqlConfigurationBuilder builder, Config config, Init? init = null)
	{
		builder.AddSqlEngine<Con, Service>(config);
		RegisterInternalServices(builder, init);
		return builder;
	}

	/// <summary>
	/// Registers SQLite with the configuration builder
	/// </summary>
	/// <param name="builder">The configuration builder to add SQLite to</param>
	/// <param name="connectionString">The connection string to use for SQLite connections</param>
	/// <param name="timeout">The default timeout for queries run against this connection</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddSQLite(this ISqlConfigurationBuilder builder, string connectionString, int timeout = 0, Init? init = null)
	{
		builder.AddSqlEngine<Con, Service>(connectionString, timeout);
		RegisterInternalServices(builder, init);
		return builder;
	}
}