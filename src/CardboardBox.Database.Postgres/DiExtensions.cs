namespace CardboardBox.Database;

using Postgres;

using Con = NpgsqlConnection;
using Service = Postgres.NpgsqlService;
using Config = ISqlConfig<NpgsqlConnection>;
using Init = Action<Postgres.IConnectionInitBuilder>;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> that configures the SQL Service and SQL generation systems
/// </summary>
public static class DiExtensions
{
	/// <summary>
	/// Registers all of the internal services with the configuration builder for Postgres
	/// </summary>
	/// <param name="bob">The configuration builder (get it? bob the builder...? Ha ha)</param>
	/// <param name="init">The initalization configuration action</param>
	public static void RegisterInternalServices(ISqlConfigurationBuilder bob, Init? init)
	{
		var provider = new ConnectionInitBuilder();
		init?.Invoke(provider);

		bob.AddInternalServices(c =>
		{
			c.AddSingleton<IConnectionInitProvider>(provider)
			 .AddSingleton<IDataSourceService, DataSourceService>();
		});

		bob.ConfigureGeneration(c =>
		{
			c.WithQueryGen<PostgresQueryGenerationService>()
			 .WithConfig(new Generation.QueryConfig
			 {
				 EscapeStart = "\"",
				 EscapeEnd = "\"",
				 ParameterCharacter = ":"
			 });
		});
	}

	/// <summary>
	/// Registers postgres with the configuration builder
	/// </summary>
	/// <typeparam name="T">The type of <see cref="ISqlConfig"/> to use</typeparam>
	/// <param name="builder">The configuration builder to add postgres to</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddPostgres<T>(this ISqlConfigurationBuilder builder, Init? init = null)
		where T : class, Config
	{
		builder.AddSqlEngine<Con, Service, T>();
		RegisterInternalServices(builder, init);
		return builder;
	}

	/// <summary>
	/// Registers postgres with the configuration builder
	/// </summary>
	/// <param name="builder">The configuration builder to add postgres to</param>
	/// <param name="config">The configuration to use for postgres connections</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddPostgres(this ISqlConfigurationBuilder builder, Config config, Init? init = null)
	{
		builder.AddSqlEngine<Con, Service>(config);
		RegisterInternalServices(builder, init);
		return builder;
	}

	/// <summary>
	/// Registers postgres with the configuration builder
	/// </summary>
	/// <param name="builder">The configuration builder to add postgres to</param>
	/// <param name="connectionString">The connection string to use for postgres connections</param>
	/// <param name="timeout">The default timeout for queries run against this connection</param>
	/// <param name="init">The optional initalization configuration action</param>
	/// <returns>The configuration builder for chaining</returns>
	public static ISqlConfigurationBuilder AddPostgres(this ISqlConfigurationBuilder builder, string connectionString, int timeout = 0, Init? init = null)
	{
		builder.AddSqlEngine<Con, Service>(connectionString, timeout);
		RegisterInternalServices(builder, init);
		return builder;
	}
}