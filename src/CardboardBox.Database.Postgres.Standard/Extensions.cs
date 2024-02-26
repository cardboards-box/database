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
                registerType.MakeGenericMethod(cls).Invoke(resolver, new object[] { typ.Name });
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
    /// Short hand for Invariant cluture ignore case
    /// </summary>
    /// <param name="first">The first string</param>
    /// <param name="second">The second string</param>
    /// <returns>Whether or not both strings are equal</returns>
    public static bool EqualsIc(this string first, string second)
    {
        return first.Equals(second, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets all of the enum values that are flagged in the given enum
    /// </summary>
    /// <typeparam name="T">The type of enum</typeparam>
    /// <param name="value">The value to get the flags from</param>
    /// <param name="onlyBits">Whether or only get bitwise flags</param>
    /// <returns>All of the activated flags</returns>
    public static IEnumerable<T> Flags<T>(this T value, bool onlyBits = false) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>();
        var ops = values.Where(x => value.HasFlag(x));

        if (onlyBits)
            ops = ops.Where(x => ((int)(object)x & ((int)(object)x - 1)) == 0);

        return ops;
    }

    /// <summary>
    /// Gets all of the flags from the given type with the given predicate
    /// </summary>
    /// <typeparam name="T">The type of enum</typeparam>
    /// <param name="value">The value to get the flags from</param>
    /// <param name="predicate">The predicate to determine if the flags should be returned</param>
    /// <returns>All of the activated flags</returns>
    public static IEnumerable<T> Flags<T>(this T value, Func<T, bool> predicate) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>();
        return values.Where(x => value.HasFlag(x) && predicate(x));
    }

    /// <summary>
    /// Gets all of the flags for the given enum
    /// </summary>
    /// <typeparam name="T">The type of enum</typeparam>
    /// <param name="_">The type to get the enums from</param>
    /// <returns>All of the flags</returns>
    public static IEnumerable<T> AllFlags<T>(this T _) where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// Shourt hand for string.Join(joiner, collection)
    /// </summary>
    /// <typeparam name="T">The type of collection</typeparam>
    /// <param name="input">The collection to combine</param>
    /// <param name="joiner">What to join the collection by</param>
    /// <returns>The combined string</returns>
    public static string StrJoin<T>(this IEnumerable<T> input, string joiner = " ")
    {
        return string.Join(joiner, input);
    }

    /// <summary>
    /// Trims the given string from the start of the input
    /// </summary>
    /// <param name="input">The input to trim from</param>
    /// <param name="trim">The string to remove</param>
    /// <returns>The trimmed string</returns>
    public static string TrimStart(this string input, string trim)
    {
        if (input.StartsWith(trim))
            return input[trim.Length..];

        return input;
    }

    /// <summary>
    /// Checks if the given datetime has expired
    /// </summary>
    /// <param name="date">The date to check</param>
    /// <param name="seconds">The seconds until expiration from now</param>
    /// <returns>Whether the date is expired</returns>
    public static bool Expired(this DateTime date, int seconds)
    {
        return date.ToUniversalTime().AddSeconds(seconds) < DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the given object inherits from the given type
    /// </summary>
    /// <typeparam name="T">The type to check for</typeparam>
    /// <param name="input">The object to check</param>
    /// <returns>Whether or not the inputted object inherits from the given type</returns>
    public static bool OfType<T>(this object input)
    {
        return input.GetType().OfType<T>();
    }

    /// <summary>
    /// Checks if the given object inherits from the given type
    /// </summary>
    /// <typeparam name="T">The type to check for</typeparam>
    /// <param name="input">The object to check</param>
    /// <param name="type">The type casted version of the input</param>
    /// <returns>Whether or not the inputted object inherits from the given type</returns>
    public static bool OfType<T>(this object input, out T? type)
    {
        if (!input.GetType().OfType<T>())
        {
            type = default;
            return false;
        }

        type = (T)input;
        return true;
    }

    /// <summary>
    /// Checks if the given type inherits from the given type
    /// </summary>
    /// <typeparam name="T">The type to check for</typeparam>
    /// <param name="type">The type to check against</param>
    /// <returns>Whether or not the inputted type inherits from the given type</returns>
    public static bool OfType<T>(this Type type)
    {
        var inf = typeof(T);
        return inf.IsAssignableFrom(type);
    }

    /// <summary>
    /// Converts the given dictionary into a query string
    /// </summary>
    /// <param name="pars">The values to parameterize</param>
    /// <returns>The outputted query string</returns>
    public static string Parameterize(this Dictionary<string, string> pars)
    {
        return pars.Count == 0
            ? string.Empty
            : "?" + string.Join("&", pars.Select(x => $"{x.Key}={x.Value}"));
    }

    /// <summary>
    /// Converts the given collection into a select many query
    /// </summary>
    /// <typeparam name="TOut">The output type</typeparam>
    /// <typeparam name="TIn">The input type</typeparam>
    /// <param name="input">The inputted collection</param>
    /// <param name="selector">What to get from the collection</param>
    /// <returns>All of the items from the inner collections</returns>
    public static async IAsyncEnumerable<TOut> SelectManyAsync<TOut, TIn>(this IEnumerable<TIn> input, Func<TIn, IAsyncEnumerable<TOut>> selector)
    {
        foreach (var item in input)
            await foreach (var output in selector(item))
                yield return output;
    }

    /// <summary>
    /// Splits the given collection into equal sized chunks
    /// </summary>
    /// <typeparam name="T">The type of collection</typeparam>
    /// <param name="data">The inputted collection</param>
    /// <param name="count">The size of the chunks</param>
    /// <returns>The outputted collection</returns>
    public static IEnumerable<T[]> Split<T>(this IEnumerable<T> data, int count)
    {
        var total = (int)Math.Ceiling((decimal)data.Count() / count);
        var current = new List<T>();

        foreach (var item in data)
        {
            current.Add(item);

            if (current.Count == total)
            {
                yield return current.ToArray();
                current.Clear();
            }
        }

        if (current.Count > 0) yield return current.ToArray();
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

    /// <summary>
    /// Moves the given iterator until if finds a selector that doesn't match
    /// </summary>
    /// <typeparam name="T">The type of data to process</typeparam>
    /// <param name="data">The iterator to process</param>
    /// <param name="previous">The last item for via any previous MoveUntil reference</param>
    /// <param name="selectors">The different properties to check against</param>
    /// <returns>All of the items in the current grouping</returns>
    public static Grouping<T> MoveUntil<T>(this IEnumerator<T> data, T? previous, params Func<T, object?>[] selectors)
    {
        var items = new List<T>();

        //Add the previous item to the collection of items
        if (previous != null) items.Add(previous);

        //Keep moving through the iterator until EoC
        while (data.MoveNext())
        {
            //Get the current item
            var current = data.Current;
            //Get the last item
            var last = items.LastOrDefault();

            //No last item? Add current and skip to next item
            if (last == null)
            {
                items.Add(current);
                continue;
            }

            //Iterate through selectors until one matches
            for (var i = 0; i < selectors.Length; i++)
            {
                //Get the keys to check
                var selector = selectors[i];
                var fir = selector(last);
                var cur = selector(current);

                //Check if the keys are the same
                var isSame = (fir == null && cur == null) ||
                    (fir != null && fir.Equals(cur));

                //They are the same, move to next selector
                if (isSame) continue;

                //Break out of the check, returning the grouped items and the last item checked
                return new(items.ToArray(), current, i);
            }

            //All selectors are the same, add item to the collection
            items.Add(current);
        }

        //Reached EoC, return items, no last, and no selector index
        return new(items.ToArray(), default, -1);
    }

    /// <summary>
    /// Fetch an index via a predicate
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="data">The data to process</param>
    /// <param name="predicate">The predicate used to find the index</param>
    /// <returns>The index or -1</returns>
    public static int IndexOf<T>(this IEnumerable<T> data, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (var item in data)
        {
            if (predicate(item))
                return index;

            index++;
        }

        return -1;
    }

    /// <summary>
    /// Fetch an index via a predicate (or null if not found)
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="data">The data to process</param>
    /// <param name="predicate">The predicate used to find the index</param>
    /// <returns>The index or null</returns>
    public static int? IndexOfNull<T>(this IEnumerable<T> data, Func<T, bool> predicate)
    {
        var idx = data.IndexOf(predicate);
        return idx == -1 ? null : idx;
    }

    /// <summary>
    /// Clones the given object via JSON serialization
    /// </summary>
    /// <typeparam name="TIn">The inputted type</typeparam>
    /// <typeparam name="TOut">The outputted type</typeparam>
    /// <param name="data">The data to clone</param>
    /// <returns>The cloned object</returns>
    public static TOut? Clone<TIn, TOut>(this TIn data) where TOut : TIn
    {
        var ser = JsonSerializer.Serialize(data);
        return JsonSerializer.Deserialize<TOut>(ser);
    }
}

/// <summary>
/// A group of items
/// </summary>
/// <typeparam name="T">The type of collection</typeparam>
/// <param name="Items">The items in this group</param>
/// <param name="Last">The last item checked (not in the current collection)</param>
/// <param name="Index">The index of the last item in the collection</param>
public record class Grouping<T>(T[] Items, T? Last, int Index);
