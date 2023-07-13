using CardboardBox.Database.Generation.CaseChange;
using Microsoft.Extensions.DependencyInjection;

namespace CardboardBox.Database.Generation;

/// <summary>
/// A helpful builder that allows for customizing how the query generation service works
/// </summary>
public interface IDependencyBuilder
{
	/// <summary>
	/// Adds a custom <see cref="IQueryConfigProvider"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryConfigProvider"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithConfig<T>() where T : class, IQueryConfigProvider;

	/// <summary>
	/// Adds a static <see cref="QueryConfig"/> for the <see cref="IQueryConfigProvider"/>
	/// </summary>
	/// <param name="config">The configuration to use</param>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithConfig(QueryConfig config);

	/// <summary>
	/// Adds a custom <see cref="ICaseChangeService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="ICaseChangeService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithCaseChange<T>() where T : class, ICaseChangeService;

	/// <summary>
	/// Adds the <see cref="CamelCaseChangeService"/> for the <see cref="ICaseChangeService"/>
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithCamelCaseChange();

	/// <summary>
	/// Adds the <see cref="PascalCaseChangeService"/> for the <see cref="ICaseChangeService"/>
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithPascalCaseChange();

	/// <summary>
	/// Adds a custom <see cref="IQueryGenerationService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryGenerationService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithQueryGen<T>() where T : class, IQueryGenerationService;

	/// <summary>
	/// Adds a custom <see cref="IQueryService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithQueryBuilder<T>() where T : class, IQueryService;

	/// <summary>
	/// Adds a custom <see cref="IReflectedService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation for the <see cref="IReflectedService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	IDependencyBuilder WithReflection<T>() where T : class, IReflectedService;
}

/// <summary>
/// A helpful builder that allows for customizing how the query generation service works
/// </summary>
public class DependencyBuilder : IDependencyBuilder
{
	private readonly IServiceCollection _services;
	private bool _queryConfig = false;
	private bool _caseChange = false;
	private bool _queryGen = false;
	private bool _querySrv = false;
	private bool _refelcted = false;

	/// <summary>
	/// A helpful builder that allows for customizing how the query generation service works
	/// </summary>
	/// <param name="services">The service collection to inject the services into</param>
	public DependencyBuilder(IServiceCollection services)
	{
		_services = services;
	}

	/// <summary>
	/// Adds a custom <see cref="IQueryConfigProvider"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryConfigProvider"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithConfig<T>() where T : class, IQueryConfigProvider
	{
		_services.AddTransient<IQueryConfigProvider, T>();
		_queryConfig = true;
		return this;
	}

	/// <summary>
	/// Adds a static <see cref="QueryConfig"/> for the <see cref="IQueryConfigProvider"/>
	/// </summary>
	/// <param name="config">The configuration to use</param>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithConfig(QueryConfig config)
	{
		_services.AddSingleton<IQueryConfigProvider>(new StaticQueryConfigProvider(config));
		_queryConfig = true;
		return this;
	}

	/// <summary>
	/// Adds a custom <see cref="ICaseChangeService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="ICaseChangeService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithCaseChange<T>() where T : class, ICaseChangeService
	{
		_services.AddTransient<ICaseChangeService, T>();
		_caseChange = true;
		return this;
	}

	/// <summary>
	/// Adds the <see cref="CamelCaseChangeService"/> for the <see cref="ICaseChangeService"/>
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithCamelCaseChange() => WithCaseChange<CamelCaseChangeService>();

	/// <summary>
	/// Adds the <see cref="PascalCaseChangeService"/> for the <see cref="ICaseChangeService"/>
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithPascalCaseChange() => WithCaseChange<PascalCaseChangeService>();

	/// <summary>
	/// Adds a custom <see cref="IQueryGenerationService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryGenerationService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithQueryGen<T>() where T : class, IQueryGenerationService
	{
		_services.AddTransient<IQueryGenerationService, T>();
		_queryGen = true;
		return this;
	}

	/// <summary>
	/// Adds a custom <see cref="IQueryService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation of the <see cref="IQueryService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithQueryBuilder<T>() where T : class, IQueryService
	{
		_services.AddTransient<IQueryService, T>();
		_querySrv = true;
		return this;
	}

	/// <summary>
	/// Adds a custom <see cref="IReflectedService"/>
	/// </summary>
	/// <typeparam name="T">The concrete implementation for the <see cref="IReflectedService"/></typeparam>
	/// <returns>The current builder for chaining</returns>
	public IDependencyBuilder WithReflection<T>() where T : class, IReflectedService
	{
		_services.AddTransient<IReflectedService, T>();
		_refelcted = true;
		return this;
	}

	/// <summary>
	/// Injects all of the necessary services Query Generation
	/// </summary>
	public void InjectServices()
	{
		if (!_queryConfig) WithConfig<NullQueryConfigProvider>();
		if (!_caseChange) WithCaseChange<NullCaseChangeService>();
		if (!_queryGen) WithQueryGen<QueryGenerationService>();
		if (!_querySrv) WithQueryBuilder<QueryService>();
		if (!_refelcted) WithReflection<ReflectedService>();
	}
}
