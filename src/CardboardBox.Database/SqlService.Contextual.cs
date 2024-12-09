namespace CardboardBox.Database;

/// <summary>
/// A contextually aware Sql Service
/// </summary>
/// <typeparam name="T">The type of SQL connection</typeparam>
public interface ISqlService<T> : ISqlService where T : IDbConnection, new() { }

/// <summary>
/// The concrete implementation of a contextually aware SQL connection
/// </summary>
/// <typeparam name="T">The type of SQL connection</typeparam>
/// <remarks>
/// Dependency Injection target constructor
/// </remarks>
/// <param name="config">The configuration for this SQL configuration</param>
public class SqlService<T>(ISqlConfig<T> config) : SqlService, ISqlService<T> where T : IDbConnection, new()
{
	/// <summary>
	/// The SQL connection configuration provider
	/// </summary>
	private readonly ISqlConfig _config = config;

	/// <summary>
	/// The connection string for this configuration
	/// </summary>
	public virtual string ConnectionString => _config.ConnectionString;

	/// <summary>
	/// The default timeout for this configuration
	/// </summary>
	public override int Timeout => _config.Timeout;

    /// <summary>
    /// Creates a new connection from the underlying ADO.Net library
    /// </summary>
    /// <returns>The created ADO.Net SQL connection</returns>
    /// <exception cref="NullReferenceException">Thrown if the created connection ends up being null</exception>
    public override Task<IDbConnection> CreateConnection()
	{
		var con = Activator.CreateInstance(typeof(T), [ConnectionString]);
		if (con == null) throw new NullReferenceException("Cannot determine connection type");
		return Task.FromResult((IDbConnection)con);
	}
}
