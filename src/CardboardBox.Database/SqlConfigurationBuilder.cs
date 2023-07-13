using Microsoft.Extensions.DependencyInjection;

namespace CardboardBox.Database;

using Generation;
using Mapping;

/// <summary>
/// Represents a builder for configuring how the <see cref="ISqlService"/>s work.
/// </summary>
public interface ISqlConfigurationBuilder
{
	/// <summary>
	/// [INTERNAL] This is used as a pass-through for the <see cref="IServiceCollection"/> for extensions.
	/// Use at your own risk.
	/// </summary>
	/// <param name="services">The service collection configuration action</param>
	/// <returns>The current builder for chaining</returns>
	ISqlConfigurationBuilder AddInternalServices(Action<IServiceCollection> services);

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <typeparam name="TConfig">The type of <see cref="ISqlConfig"/> to register</typeparam>
	/// <returns>The current builder for chaining</returns>
	ISqlConfigurationBuilder AddSqlEngine<TConnection, TService, TConfig>()
		where TConnection : IDbConnection, new()
		where TService : class, ISqlService<TConnection>
		where TConfig : class, ISqlConfig<TConnection>;

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <param name="config">The configuration to use for this connection</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder AddSqlEngine<TConnection, TService>(ISqlConfig<TConnection> config)
		where TConnection : IDbConnection, new()
		where TService : class, ISqlService<TConnection>;

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <param name="connectionString">The connection string to use for this configuration</param>
	/// <param name="timeout">The default timeout to use for queries run against this configuration</param>
	/// <returns>The current builder for chaining</returns>
	ISqlConfigurationBuilder AddSqlEngine<TConnection, TService>(string connectionString, int timeout = 0)
		where TConnection : IDbConnection, new()
		where TService : class, ISqlService<TConnection>;

	/// <summary>
	/// Configures types and mappings for Dapper Fluent Map
	/// </summary>
	/// <param name="config">The action used to configure types</param>
	/// <returns>The current builder for chaining</returns>
	ISqlConfigurationBuilder ConfigureTypes(Action<ITypeMapBuilder> config);

	/// <summary>
	/// Configures query generation
	/// </summary>
	/// <param name="config">The action used to configure generation</param>
	/// <returns>The current builder for chaining</returns>
	ISqlConfigurationBuilder ConfigureGeneration(Action<IDependencyBuilder> config);
}

/// <summary>
/// A builder for configuring how the <see cref="ISqlService"/>s work.
/// </summary>
public class SqlConfigurationBuilder : ISqlConfigurationBuilder
{
	private readonly IServiceCollection _services;
	private readonly TypeMapBuilder _types;
	private readonly DependencyBuilder _generation;

	private Action? _lastService;

	/// <summary>
	/// A builder for configuring how the <see cref="ISqlService"/>s work.
	/// </summary>
	/// <param name="services">The service collection to register the services on</param>
	public SqlConfigurationBuilder(IServiceCollection services)
	{
		_services = services;
		_types = new TypeMapBuilder();
		_generation = new DependencyBuilder(services);
	}

	/// <summary>
	/// [INTERNAL] This is used as a pass-through for the <see cref="IServiceCollection"/> for extensions.
	/// Use at your own risk.
	/// </summary>
	/// <param name="services">The service collection configuration action</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder AddInternalServices(Action<IServiceCollection> services)
	{
		services?.Invoke(_services);
		return this;
	}

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <typeparam name="TConfig">The type of <see cref="ISqlConfig"/> to register</typeparam>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder AddSqlEngine<TConnection, TService, TConfig>()
		where TConnection: IDbConnection, new()
		where TService : class, ISqlService<TConnection>
		where TConfig : class, ISqlConfig<TConnection>
	{
		_services
			.AddTransient<ISqlService<TConnection>, TService>()
			.AddTransient<ISqlConfig<TConnection>, TConfig>();

		_lastService = () => _services.AddTransient<ISqlService, TService>();

		return this;
	}

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <param name="config">The configuration to use for this connection</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder AddSqlEngine<TConnection, TService>(ISqlConfig<TConnection> config)
		where TConnection : IDbConnection, new()
		where TService : class, ISqlService<TConnection>
	{
		_services
			.AddTransient<ISqlService<TConnection>, TService>()
			.AddSingleton(config);

		_lastService = () => _services.AddTransient<ISqlService, TService>();

		return this;
	}

	/// <summary>
	/// Adds a SQL engine to the services
	/// </summary>
	/// <typeparam name="TConnection">The type of <see cref="IDbConnection"/> to register</typeparam>
	/// <typeparam name="TService">The type of <see cref="ISqlService"/> to register</typeparam>
	/// <param name="connectionString">The connection string to use for this configuration</param>
	/// <param name="timeout">The default timeout to use for queries run against this configuration</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder AddSqlEngine<TConnection, TService>(string connectionString, int timeout = 0)
		where TConnection : IDbConnection, new()
		where TService : class, ISqlService<TConnection>
	{
		return AddSqlEngine<TConnection, TService>(new SqlConfig<TConnection>
		{
			ConnectionString = connectionString,
			Timeout = timeout
		});
	}

	/// <summary>
	/// Configures types and mappings for Dapper Fluent Map
	/// </summary>
	/// <param name="config">The action used to configure types</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder ConfigureTypes(Action<ITypeMapBuilder> config)
	{
		config?.Invoke(_types);
		return this;
	}

	/// <summary>
	/// Configures query generation
	/// </summary>
	/// <param name="config">The action used to configure generation</param>
	/// <returns>The current builder for chaining</returns>
	public ISqlConfigurationBuilder ConfigureGeneration(Action<IDependencyBuilder> config)
	{
		config?.Invoke(_generation);
		return this;
	}

	/// <summary>
	/// Completes registration of all SQL services
	/// </summary>
	/// <returns>The service collection for chaining</returns>
	public void Register()
	{
		_types.Init();
		_generation.InjectServices();
		_lastService?.Invoke();
	}
}
