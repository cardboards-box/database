namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a cache service for <see cref="ReflectedType"/>s
/// </summary>
public interface IReflectedService
{
	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given generic
	/// </summary>
	/// <typeparam name="T">The type of class to get</typeparam>
	/// <returns>The <see cref="ReflectedType"/> for the given generic</returns>
	ReflectedType GetType<T>();

	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given type
	/// </summary>
	/// <param name="type">The type of class to get</param>
	/// <returns>The <see cref="ReflectedType"/> for the given type</returns>
	ReflectedType GetType(Type type);
}

/// <summary>
/// A service that fetches and caches the <see cref="ReflectedType"/>s of varying models
/// </summary>
public class ReflectedService : IReflectedService
{
	private readonly Dictionary<string, ReflectedType> _cache = new();

	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given generic
	/// </summary>
	/// <typeparam name="T">The type of class to get</typeparam>
	/// <returns>The <see cref="ReflectedType"/> for the given generic</returns>
	public ReflectedType GetType<T>() => GetType(typeof(T));

	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given type
	/// </summary>
	/// <param name="type">The type of class to get</param>
	/// <returns>The <see cref="ReflectedType"/> for the given type</returns>
	public ReflectedType GetType(Type type)
	{
		var name = type.FullName ?? type.Name;

		if (_cache.TryGetValue(name, out var value))
			return value;

		var table = type.GetCustomAttribute<TableAttribute>();
		var props = type.GetProperties()
			.Select(t => new ReflectedProperty(t, t.GetCustomAttribute<ColumnAttribute>()))
			.Where(t => !t.Ignore)
			.ToDictionary(t => t.Property.Name);

		return _cache[name] = new(type, table, props);
	}
}
