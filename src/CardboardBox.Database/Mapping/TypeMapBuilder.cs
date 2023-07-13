using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using Dapper.FluentMap.Conventions;

namespace CardboardBox.Database.Mapping;

using Fluent = Action<FluentMapConfiguration>;

/// <summary>
/// Represents a builder that allows for configuring how the Dapper Fluent Map system works
/// </summary>
public interface ITypeMapBuilder
{
	/// <summary>
	/// Adds a case convention and it's subsequent types
	/// </summary>
	/// <typeparam name="T">The type of convention to map</typeparam>
	/// <returns>A configuration builder for entities mapped to this convention</returns>
	IConventionBuilder CaseConvention<T>() where T : Convention, new();

	/// <summary>
	/// Configuration builder for Dapper Fluent camel_case conventions
	/// </summary>
	/// <returns>A configuration builder for entities mapped to this convention</returns>
	IConventionBuilder CamelCase();

	/// <summary>
	/// Tells the dapper mapper how to handle a specific type. 
	/// This is for polyfilling missing types like <see cref="DateTime"/> and <see cref="bool"/> in older versions of SQLite
	/// </summary>
	/// <typeparam name="T">The type to map</typeparam>
	/// <typeparam name="THandler">The type handler to use</typeparam>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder TypeHandler<T, THandler>() where THandler : SqlMapper.TypeHandler<T>, new();

	/// <summary>
	/// Polyfills handling <see cref="bool"/> values in older versions of SQLite
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder PolyfillBooleanHandler();

	/// <summary>
	/// Polyfills handling <see cref="DateTime"/> values
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder PolyfillDateTimeHandler();

	/// <summary>
	/// Adds a handler for serializing a generic array to string
	/// </summary>
	/// <typeparam name="T">The type of generic array</typeparam>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder ArrayHandler<T>();

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder JsonHandler<T>() where T : new();

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder NullableJsonHandler<T>();

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <param name="default">How to create a default object</param>
	/// <returns>The current builder for chaining</returns>
	ITypeMapBuilder DefaultJsonHandler<T>(Func<T> @default);
}

/// <summary>
/// A builder that allows for configuring how the Dapper Fluent Map system works
/// </summary>
public class TypeMapBuilder : ITypeMapBuilder
{
	private readonly List<Fluent> _conventions = new();

	/// <summary>
	/// Adds a case convention and it's subsequent types
	/// </summary>
	/// <typeparam name="T">The type of convention to map</typeparam>
	/// <returns>A configuration builder for entities mapped to this convention</returns>
	public IConventionBuilder CaseConvention<T>() where T : Convention, new()
	{
		var bob = new ConventionBuilder();

		_conventions.Add((c) =>
		{
			var con = c.AddConvention<T>();

			foreach (var map in bob.Maps)
				map(con);
		});

		return bob;
	}

	/// <summary>
	/// Configuration builder for Dapper Fluent camel_case conventions
	/// </summary>
	/// <returns>A configuration builder for entities mapped to this convention</returns>
	public IConventionBuilder CamelCase() => CaseConvention<CamelCaseMap>();

	/// <summary>
	/// Tells the dapper mapper how to handle a specific type. 
	/// This is for polyfilling missing types like <see cref="DateTime"/> and <see cref="bool"/> in older versions of SQLite
	/// </summary>
	/// <typeparam name="T">The type to map</typeparam>
	/// <typeparam name="THandler">The type handler to use</typeparam>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder TypeHandler<T, THandler>() where THandler : SqlMapper.TypeHandler<T>, new()
	{
		return TypeHandler(new THandler());
	}

	/// <summary>
	/// Tells the dapper mapper how to handle a specific type. 
	/// This is for polyfilling missing types like <see cref="DateTime"/> and <see cref="bool"/> in older versions of SQLite
	/// </summary>
	/// <typeparam name="T">The type to map</typeparam>
	/// <param name="handler">The type handler to use</param>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder TypeHandler<T>(SqlMapper.TypeHandler<T> handler)
	{
		SqlMapper.RemoveTypeMap(typeof(T));
		SqlMapper.AddTypeHandler(handler);
		return this;
	}

	/// <summary>
	/// Polyfills handling <see cref="bool"/> values in older versions of SQLite
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder PolyfillBooleanHandler()
	{
		return TypeHandler<bool, BooleanHandler>()
			.TypeHandler<bool?, NullableBooleanHandler>();
	}

	/// <summary>
	/// Polyfills handling <see cref="DateTime"/> values
	/// </summary>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder PolyfillDateTimeHandler()
	{
		return TypeHandler<DateTime, DateTimeHandler>()
			.TypeHandler<DateTime?, NullableDateTimeHandler>();
	}

	/// <summary>
	/// Adds a handler for serializing a generic array to string
	/// </summary>
	/// <typeparam name="T">The type of generic array</typeparam>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder ArrayHandler<T>() => DefaultJsonHandler(Array.Empty<T>);

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder JsonHandler<T>() where T: new() => TypeHandler(new JsonHandler<T>());

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder NullableJsonHandler<T>() => TypeHandler(new NullableJsonHandler<T>());

	/// <summary>
	/// Adds a handler for serializing generic objects
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	/// <param name="default">How to create a default object</param>
	/// <returns>The current builder for chaining</returns>
	public ITypeMapBuilder DefaultJsonHandler<T>(Func<T> @default) => TypeHandler(new DefaultJsonHandler<T>(@default));

	/// <summary>
	/// Intiailizes the Dapper Fluent Map
	/// </summary>
	public void Init()
	{
		FluentMapper.Initialize(config =>
		{
			foreach (var con in _conventions)
				con.Invoke(config);
		});
	}
}
