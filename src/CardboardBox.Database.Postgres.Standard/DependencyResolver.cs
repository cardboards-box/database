using Serilog;

namespace CardboardBox.Database.Postgres.Standard;

/// <summary>
/// A dependency builder for easily registering services, models, and other configuration options
/// </summary>
public interface IDependencyResolver
{
    /// <summary>
    /// Add the services to the dependency resolver
    /// </summary>
    /// <param name="services">The service collection configuration action</param>
    /// <returns></returns>
    IDependencyResolver AddServices(Action<IServiceCollection> services);

    /// <summary>
    /// Add the services to the dependency resolver 
    /// </summary>
    /// <param name="services">The service collection configuration action</param>
    /// <returns></returns>
    IDependencyResolver AddServices(Action<IServiceCollection, IConfiguration> services);

    /// <summary>
    /// Add the services to the dependency resolver 
    /// </summary>
    /// <param name="services">The service collection configuration action</param>
    /// <returns></returns>
    IDependencyResolver AddServices(Func<IServiceCollection, Task> services);

    /// <summary>
    /// Add the services to the dependency resolver 
    /// </summary>
    /// <param name="services">The service collection configuration action</param>
    /// <returns></returns>
    IDependencyResolver AddServices(Func<IServiceCollection, IConfiguration, Task> services);

    /// <summary>
    /// Registers a database model with the dependency resolver
    /// </summary>
    /// <typeparam name="T">The type of database model</typeparam>
    /// <returns></returns>
    IDependencyResolver Model<T>();

    /// <summary>
    /// Registers a type table model with the dependency resolver
    /// </summary>
    /// <typeparam name="T">The type of table type model</typeparam>
    /// <param name="name">The name of the table type in the database</param>
    /// <returns></returns>
    IDependencyResolver Type<T>(string? name = null);

    /// <summary>
    /// Adds a Dapper convention for handling JSON data for the given type
    /// </summary>
    /// <typeparam name="T">The type to register</typeparam>
    /// <param name="default">Results the json model default</param>
    /// <returns></returns>
    IDependencyResolver JsonModel<T>(Func<T> @default);

    /// <summary>
    /// Adds a Dapper convention for handling JSON data for the given type
    /// </summary>
    /// <typeparam name="T">The type to register</typeparam>
    /// <returns></returns>
    IDependencyResolver JsonModel<T>();

    /// <summary>
    /// Adds a Dapper convention for handling custom mappings
    /// </summary>
    /// <param name="mapping">The mapping to register</param>
    /// <returns></returns>
    IDependencyResolver Mapping(Action<ITypeMapBuilder> mapping);

    /// <summary>
    /// Adds a Dapper convention for handling custom model conventions
    /// </summary>
    /// <param name="convention">The convention to register</param>
    /// <returns></returns>
    IDependencyResolver Convention(Action<IConventionBuilder> convention);

    /// <summary>
    /// Adds a way of manipulating the Postgres Data Source Builder
    /// </summary>
    /// <param name="connections">The manipulator</param>
    /// <returns></returns>
    IDependencyResolver Connections(Action<NpgsqlDataSourceBuilder> connections);

    /// <summary>
    /// Adds a transient service to the dependency resolver
    /// </summary>
    /// <typeparam name="TService">The interface for the service</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation for the service</typeparam>
    /// <returns></returns>
    IDependencyResolver Transient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// Adds a singleton service to the dependency resolver
    /// </summary>
    /// <typeparam name="TService">The interface for the service</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation for the service</typeparam>
    /// <returns></returns>
    IDependencyResolver Singleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// Adds a singleton service to the dependency resolver
    /// </summary>
    /// <typeparam name="TService">The interface for the service</typeparam>
    /// <param name="instance">The instance of the interface</param>
    /// <returns></returns>
    IDependencyResolver Singleton<TService>(TService instance)
        where TService : class;

    /// <summary>
    /// Adds a configuration method for the serilog logger
    /// </summary>
    /// <param name="logger">The logger configuration action</param>
    /// <returns></returns>
    IDependencyResolver Logger(Action<LoggerConfiguration> logger);
}

internal class DependencyResolver : IDependencyResolver
{
    private readonly List<Func<IServiceCollection, IConfiguration, Task>> _services = new();
    private readonly List<Action<IConventionBuilder>> _conventions = new();
    private readonly List<Action<ITypeMapBuilder>> _dbMapping = new();
    private readonly List<Action<NpgsqlDataSourceBuilder>> _connections = new();
    private readonly List<Action<LoggerConfiguration>> _loggers = new();

    public IDependencyResolver AddServices(Action<IServiceCollection> services)
    {
        return AddServices((s, _) => services(s));
    }

    public IDependencyResolver AddServices(Action<IServiceCollection, IConfiguration> services)
    {
        return AddServices((s, c) =>
        {
            services(s, c);
            return Task.CompletedTask;
        });
    }

    public IDependencyResolver AddServices(Func<IServiceCollection, Task> services)
    {
        return AddServices((s, _) => services(s));
    }

    public IDependencyResolver AddServices(Func<IServiceCollection, IConfiguration, Task> services)
    {
        _services.Add(services);
        return this;
    }

    public IDependencyResolver Model<T>()
    {
        return Convention(x => x.Entity<T>());
    }

    public IDependencyResolver Type<T>(string? name = null)
    {
        return Connections(x => x.MapComposite<T>(name));
    }

    public IDependencyResolver JsonModel<T>(Func<T> @default)
    {
        return Mapping(x => x.DefaultJsonHandler(@default));
    }

    public IDependencyResolver Connections(Action<NpgsqlDataSourceBuilder> connections)
    {
        _connections.Add(connections);
        return this;
    }

    public IDependencyResolver Mapping(Action<ITypeMapBuilder> mapping)
    {
        _dbMapping.Add(mapping);
        return this;
    }

    public IDependencyResolver Convention(Action<IConventionBuilder> convention)
    {
        _conventions.Add(convention);
        return this;
    }

    public IDependencyResolver JsonModel<T>() => JsonModel<T?>(() => default);

    public IDependencyResolver Transient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        return AddServices(x => x.AddTransient<TService, TImplementation>());
    }

    public IDependencyResolver Singleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        return AddServices(x => x.AddSingleton<TService, TImplementation>());
    }

    public IDependencyResolver Singleton<TService>(TService instance)
        where TService : class
    {
        return AddServices(x => x.AddSingleton(instance));
    }

    public IDependencyResolver Logger(Action<LoggerConfiguration> logger)
    {
        _loggers.Add(logger);
        return this;
    }

    public async Task RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services
            .AddJson(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            })
            .AddCardboardHttp()
            .AddSerilog(c =>
            {
                _loggers.Each(x => x(c));
                c
                 .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Error)
                 .MinimumLevel.Override("Microsoft.Extensions.Http.DefaultHttpClientFactory", Serilog.Events.LogEventLevel.Error)
                 .WriteTo.Console()
                 .WriteTo.File(Path.Combine("logs", "log.txt"), rollingInterval: RollingInterval.Day)
                 .MinimumLevel.Debug();
            });

        foreach (var action in _services)
            await action(services, config);
    }

    public void RegisterDatabase(IServiceCollection services, string scriptDir)
    {
        services
            .AddSqlService(c =>
            {
                c.ConfigureGeneration(a => a.WithCamelCaseChange())
                 .ConfigureTypes(a =>
                 {
                     var conv = a.CamelCase();
                     foreach (var convention in _conventions)
                         convention(conv);

                     foreach (var mapping in _dbMapping)
                         mapping(a);
                 });

                c.AddPostgres<SqlConfig>(a =>
                {
                    a.OnCreate(con =>
                    {
                        _connections.Each(act => act(con));
                        return Task.CompletedTask;
                    });
                    a.OnInit(con => new DatabaseDeploy(con, scriptDir).ExecuteScripts());
                });
            });
    }

    public Task Build(IServiceCollection services, IConfiguration config, string scriptDir = "Scripts")
    {
        RegisterDatabase(services, scriptDir);
        return RegisterServices(services, config);
    }
}