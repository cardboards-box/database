namespace CardboardBox.Database;

/// <summary>
/// Represents a configuration for a SQL connection
/// </summary>
public interface ISqlConfig
{
	/// <summary>
	/// The connection string
	/// </summary>
	string ConnectionString { get; }

	/// <summary>
	/// The default timeout for the SQL queries
	/// </summary>
	int Timeout { get; }
}

/// <summary>
/// A contextually aware configuration for a SQL connection
/// </summary>
/// <typeparam name="T">The type of SQL connection expected</typeparam>
public interface ISqlConfig<T> : ISqlConfig where T : IDbConnection { }

/// <summary>
/// The concrete implementation of the contextually aware configuration
/// </summary>
/// <typeparam name="T">The type of SQL connection expected</typeparam>
public class SqlConfig<T> : SqlConfig, ISqlConfig<T> where T: IDbConnection { }

/// <summary>
/// The concrete implementation for the SQL configuration
/// </summary>
public class SqlConfig : ISqlConfig
{
	/// <summary>
	/// The connection string
	/// </summary>
	public string ConnectionString { get; set; } = "";

	/// <summary>
	/// The default timeout for the SQL queries
	/// </summary>
	public int Timeout { get; set; } = 0;
}
