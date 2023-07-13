using Microsoft.Extensions.DependencyInjection;

namespace CardboardBox.Database;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> that configures the SQL Service and SQL generation systems
/// </summary>
public static class DiExtensions
{
	/// <summary>
	/// Starts the registration of all of the Sql Services. This should only be called once per <see cref="IServiceCollection"/>
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to register against</param>
	/// <param name="config">The configuration action</param>
	/// <returns>The service collection for chaining</returns>
	public static IServiceCollection AddSqlService(this IServiceCollection services, Action<ISqlConfigurationBuilder> config)
	{
		var bob = new SqlConfigurationBuilder(services);
		config?.Invoke(bob);
		bob.Register();
		return services;
	}
}
