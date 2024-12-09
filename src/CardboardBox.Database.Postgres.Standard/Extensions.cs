namespace CardboardBox.Database.Postgres.Standard;

using Attributes;
using Orms;

/// <summary>
/// Helpful extensions for stuff and things
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Creates a dependency resolver and attaches it to the given service collection
    /// </summary>
    /// <param name="services">The service collection to attach to</param>
    /// <param name="config">The configuration for the application</param>
    /// <param name="configure">The configuration action for the application services</param>
    /// <param name="assemblies">The assemblies to scan for types</param>
    /// <returns></returns>
    public static Task AddServices(this IServiceCollection services, IConfiguration config, Action<IDependencyResolver> configure, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var bob = new DependencyResolver();
        configure(bob
            .RegisterModels(assemblies)
            .Transient<IOrmService, OrmService>());
        return bob.Build(services, config);
    }

    /// <summary>
    /// Registers all class types with the given attributes:
    /// <see cref="CompositeAttribute"/>, <see cref="TableAttribute"/>, <see cref="TypeAttribute"/>
    /// This is handled automatically if you use <see cref="AddServices(IServiceCollection, IConfiguration, Action{IDependencyResolver}, Assembly[])"/>
    /// </summary>
    /// <param name="resolver">The dependency resolve to attach to</param>
    /// <param name="assemblies">The assemblies to scan for types</param>
    /// <returns>The dependency resolver for chaining</returns>
    public static IDependencyResolver RegisterModels(this IDependencyResolver resolver, params Assembly[] assemblies)
    {
        var modelAttributes = new[] { typeof(CompositeAttribute), typeof(TableAttribute) };

        var types = assemblies.SelectMany(t => t.GetTypes());

        var resolverType = typeof(DependencyResolver);
        var registerModel = resolverType.GetMethod(nameof(DependencyResolver.Model));
        var registerType = resolverType.GetMethod(nameof(DependencyResolver.Type));
        if (registerModel is null || registerType is null) return resolver;

        var classes = types.Where(t => t.IsClass && !t.IsAbstract);
        foreach (var cls in classes)
        {
            if (modelAttributes.Any(a => cls.GetCustomAttribute(a) is not null))
                registerModel.MakeGenericMethod(cls).Invoke(resolver, null);

            foreach (var typ in cls.GetCustomAttributes<TypeAttribute>())
                registerType.MakeGenericMethod(cls).Invoke(resolver, [typ.Name]);
        }

        return resolver;
    }

    /// <summary>
    /// Adds a <see cref="IDbInterjectService"/> to the dependency resolver
    /// </summary>
    /// <typeparam name="T">The instance of the interject service</typeparam>
    /// <param name="resolver">The dependency resolver</param>
    /// <returns>The dependency resolver for chaining</returns>
    public static IDependencyResolver Inject<T>(this IDependencyResolver resolver) where T : class, IDbInterjectService
    {
        return resolver.Transient<IDbInterjectService, T>();
    }

    /// <summary>
    /// Downloads the given url and returns the stream and path (works with discord)
    /// </summary>
    /// <param name="api">The API service to piggy back off of</param>
    /// <param name="url">The URL to download from</param>
    /// <returns>The downloaded stream and path</returns>
    /// <exception cref="NullReferenceException">Thrown if the request is invalid</exception>
    public static async Task<(MemoryStream stream, string path)> Download(this IApiService api, string url)
    {
        var result = await api.Create(url, "GET")
            .With(c =>
            {
                c.Headers.Add("Cache-Control", "no-cache");
                c.Headers.Add("Cache-Control", "no-store");
                c.Headers.Add("Cache-Control", "max-age=1");
                c.Headers.Add("Cache-Control", "s-maxage=1");
                c.Headers.Add("Pragma", "no-cache");
            })
            .Result() ?? throw new NullReferenceException("Http result was null for down: " + url);

        result.EnsureSuccessStatusCode();

        var headers = result.Content.Headers;
        var path = headers?.ContentDisposition?.FileName ?? headers?.ContentDisposition?.Parameters?.FirstOrDefault()?.Value ?? "";

        var io = new MemoryStream();
        using (var stream = await result.Content.ReadAsStreamAsync())
            await stream.CopyToAsync(io);

        io.Position = 0;
        return (io, path);
    }
}

