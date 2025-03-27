namespace CardboardBox.Database.Postgres.Standard;

/// <summary>
/// Represents a cache item that resolves only once when resolving
/// </summary>
/// <typeparam name="T">The type of item being cached</typeparam>
/// <param name="Resolver">The resolver to get the latest instance of the cached item</param>
/// <param name="ExpireMinutes">How many minutes the cache should live for</param>
public record class CacheItem<T>(
    Func<Task<T>> Resolver,
    double ExpireMinutes = 5.0)
{
    /// <summary>
    /// The cached item
    /// </summary>
    public T? Cache { get; private set; }

    /// <summary>
    /// The current resolver that is to be run
    /// </summary>
    public Task<T>? CurrentResolver { get; private set; }

    /// <summary>
    /// When the cache will expire
    /// </summary>
    public DateTime? Expires { get; private set; }

    /// <summary>
    /// Whether or not the cache is valid
    /// </summary>
    public bool Expired => !(Cache is not null && Expires.HasValue && Expires.Value > DateTime.Now);

    /// <summary>
    /// Resolves the item from either the cache or the latest depending on the last time the cache was taken
    /// </summary>
    public async Task<T?> Get()
    {
        //Cache is valid, skip resolving
        if (!Expired) return Cache;
        //If we are already resolving, return the current resolver
        if (CurrentResolver is not null) return await CurrentResolver!;
        //Fetch the latest and cache it
        Cache = await (CurrentResolver = Resolver());
        Expires = DateTime.Now.AddMinutes(ExpireMinutes);
        CurrentResolver = null;
        return Cache;
    }

    /// <summary>
    /// Clear the current cache and ensure the next request will re-fetch
    /// </summary>
    public void Bust()
    {
        Cache = default;
        Expires = null;
    }
}