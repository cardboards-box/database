using Dapper.FluentMap.Configuration;

namespace CardboardBox.Database.Mapping;

using Fluent = Action<FluentConventionConfiguration>;

/// <summary>
/// Represents a builder for creating Dapper fluent mappings
/// </summary>
public interface IConventionBuilder
{
	/// <summary>
	/// Runs the given action against the configuration builder
	/// </summary>
	/// <param name="config">The action to run</param>
	/// <returns>The current builder for chaining</returns>
	IConventionBuilder Map(Fluent config);

	/// <summary>
	/// Maps the given entity in the configuration builder
	/// </summary>
	/// <typeparam name="T">The type of POCO to map</typeparam>
	/// <returns>The current builder for chaining</returns>
	IConventionBuilder Entity<T>();
}

/// <summary>
/// A builder for creating Dapper fluent mappings
/// </summary>
public class ConventionBuilder : IConventionBuilder
{
	private readonly List<Fluent> _entityMaps = new();

	/// <summary>
	/// All of the maps this builder created
	/// </summary>
	public Fluent[] Maps => _entityMaps.ToArray();

	/// <summary>
	/// Runs the given action against the configuration builder
	/// </summary>
	/// <param name="config">The action to run</param>
	/// <returns>The current builder for chaining</returns>
	public IConventionBuilder Map(Fluent config)
	{
		_entityMaps.Add(config);
		return this;
	}

	/// <summary>
	/// Maps the given entity in the configuration builder
	/// </summary>
	/// <typeparam name="T">The type of POCO to map</typeparam>
	/// <returns>The current builder for chaining</returns>
	public IConventionBuilder Entity<T>()
	{
		return Map(c => c.ForEntity<T>());
	}
}
