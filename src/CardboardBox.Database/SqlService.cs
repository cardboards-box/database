namespace CardboardBox.Database;

/// <summary>
/// A wrapper for Dapper SQL connections
/// </summary>
public interface ISqlService
{
	/// <summary>
	/// The default timeout for any SQL query executed. Can be override on a per request basis.
	/// </summary>
	int Timeout { get; }

	/// <summary>
	/// Creates a new connection from the underlying ADO.Net library
	/// </summary>
	/// <returns>The created ADO.Net SQL connection</returns>
	Task<IDbConnection> CreateConnection();

	/// <summary>
	/// Fetches a single record from the database
	/// </summary>
	/// <typeparam name="T">The type to map the record to</typeparam>
	/// <param name="query">The query to run to fetch the record</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>A single record or null from the database</returns>
	Task<T?> Fetch<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null);

	/// <summary>
	/// Fetches mutliple records from the database
	/// </summary>
	/// <typeparam name="T">The type to map the record to</typeparam>
	/// <param name="query">The query to run to fetch the records</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>A multiple records or an empty array from the database</returns>
	Task<T[]> Get<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null);

	/// <summary>
	/// Executes the given query and returns the record count
	/// </summary>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>The number of records that were modified by the query (or whatever the return code of the query was)</returns>
	Task<int> Execute(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null);

	/// <summary>
	/// Executes the given query and returns the scalar result
	/// </summary>
	/// <typeparam name="T">The type of scalar return result</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>The return result of the query</returns>
	Task<T> ExecuteScalar<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null);

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <typeparam name="T3">The third POCO to map</typeparam>
	/// <typeparam name="T4">The forth POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	Task<(T1 item1, T2 item2, T3 item3, T4 item4)[]> QueryTupleAsync<T1, T2, T3, T4>(string query, object? parameters = null, string splitOn = "split");

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <typeparam name="T3">The third POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	Task<(T1 item1, T2 item2, T3 item3)[]> QueryTupleAsync<T1, T2, T3>(string query, object? parameters = null, string splitOn = "split");

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	Task<(T1 item1, T2 item2)[]> QueryTupleAsync<T1, T2>(string query, object? parameters = null, string splitOn = "split");

	/// <summary>
	/// Provides an easy way to fetching paginated results in one query
	/// </summary>
	/// <typeparam name="T">The type of record to return</typeparam>
	/// <param name="query">The paginated query</param>
	/// <param name="parameters">The parameters to execute with</param>
	/// <param name="page">The page of results you want</param>
	/// <param name="size">The size of each page</param>
	/// <param name="offsetName">The name of the offset parameter in the query</param>
	/// <param name="sizeName">The name of the size parameter in the query</param>
	/// <returns>The paginated query results</returns>
	Task<PaginatedResult<T>> Paginate<T>(string query, object? parameters = null, int page = 1, int size = 100, string offsetName = "offset", string sizeName = "limit");
}

/// <summary>
/// Default implementation for the Dapper wrapper (see <see cref="ISqlService"/>)
/// </summary>
public abstract class SqlService : ISqlService
{
	/// <summary>
	/// The default timeout for any SQL query executed. Can be override on a per request basis.
	/// </summary>
	public abstract int Timeout { get; }

	/// <summary>
	/// Creates a new connection from the underlying ADO.Net library
	/// </summary>
	/// <returns>The created ADO.Net SQL connection</returns>
	public abstract Task<IDbConnection> CreateConnection();

	/// <summary>
	/// Fetches a single record from the database
	/// </summary>
	/// <typeparam name="T">The type to map the record to</typeparam>
	/// <param name="query">The query to run to fetch the record</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>A single record or null from the database</returns>
	public virtual async Task<T?> Fetch<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null)
	{
		using var con = await CreateConnection();
		return await con.QueryFirstOrDefaultAsync<T>(query, parameters, transaction, timeout ?? Timeout);
	}

	/// <summary>
	/// Fetches mutliple records from the database
	/// </summary>
	/// <typeparam name="T">The type to map the record to</typeparam>
	/// <param name="query">The query to run to fetch the records</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>A multiple records or an empty array from the database</returns>
	public virtual async Task<T[]> Get<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null)
	{
		using var con = await CreateConnection();
		return (await con.QueryAsync<T>(query, parameters, transaction, timeout ?? Timeout)).ToArray();
	}

	/// <summary>
	/// Executes the given query and returns the record count
	/// </summary>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>The number of records that were modified by the query (or whatever the return code of the query was)</returns>
	public virtual async Task<int> Execute(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null)
	{
		using var con = await CreateConnection();
		return await con.ExecuteAsync(query, parameters, transaction, timeout ?? Timeout);
	}

	/// <summary>
	/// Executes the given query and returns the scalar result
	/// </summary>
	/// <typeparam name="T">The type of scalar return result</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to run the query with</param>
	/// <param name="timeout">An optional timeout (overrides the <see cref="Timeout"/> for this request)</param>
	/// <param name="transaction">An optional transaction to run the query within</param>
	/// <returns>The return result of the query</returns>
	public virtual async Task<T> ExecuteScalar<T>(string query, object? parameters = null, int? timeout = null, IDbTransaction? transaction = null)
	{
		using var con = await CreateConnection();
		return await con.ExecuteScalarAsync<T>(query, parameters, transaction, timeout ?? Timeout);
	}

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <typeparam name="T3">The third POCO to map</typeparam>
	/// <typeparam name="T4">The forth POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	public async Task<(T1 item1, T2 item2, T3 item3, T4 item4)[]> QueryTupleAsync<T1, T2, T3, T4>(string query, object? parameters = null, string splitOn = "split")
	{
		using var con = await CreateConnection();
		return (await con.QueryAsync<T1, T2, T3, T4, (T1, T2, T3, T4)>(query,
			(a, b, c, d) => (a, b, c, d),
			parameters,
			splitOn: splitOn)).ToArray();
	}

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <typeparam name="T3">The third POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	public async Task<(T1 item1, T2 item2, T3 item3)[]> QueryTupleAsync<T1, T2, T3>(string query, object? parameters = null, string splitOn = "split")
	{
		using var con = await CreateConnection();
		return (await con.QueryAsync<T1, T2, T3, (T1, T2, T3)>(query,
			(a, b, c) => (a, b, c),
			parameters,
			splitOn: splitOn)).ToArray();
	}

	/// <summary>
	/// Provides an easy way of returning multiple POCOs worth of data in one query.
	/// </summary>
	/// <typeparam name="T1">The first POCO to map</typeparam>
	/// <typeparam name="T2">The second POCO to map</typeparam>
	/// <param name="query">The query to execute</param>
	/// <param name="parameters">The parameters to execute the query with</param>
	/// <param name="splitOn">Which column name(s) to separate the return results on (Defaults to "split")</param>
	/// <returns>A tuple containing the return results of the query</returns>
	public async Task<(T1 item1, T2 item2)[]> QueryTupleAsync<T1, T2>(string query, object? parameters = null, string splitOn = "split")
	{
		using var con = await CreateConnection();
		return (await con.QueryAsync<T1, T2, (T1, T2)>(query,
			(a, b) => (a, b),
			parameters,
			splitOn: splitOn)).ToArray();
	}

	/// <summary>
	/// Provides an easy way to fetching paginated results in one query
	/// </summary>
	/// <typeparam name="T">The type of record to return</typeparam>
	/// <param name="query">The paginated query</param>
	/// <param name="parameters">The parameters to execute with</param>
	/// <param name="page">The page of results you want</param>
	/// <param name="size">The size of each page</param>
	/// <param name="offsetName">The name of the offset parameter in the query</param>
	/// <param name="sizeName">The name of the size parameter in the query</param>
	/// <returns>The paginated query results</returns>
	public async Task<PaginatedResult<T>> Paginate<T>(string query, object? parameters = null, int page = 1, int size = 100, string offsetName = "offset", string sizeName = "limit")
	{
		var p = new DynamicParameters(parameters);
		p.Add(offsetName, (page - 1) * size);
		p.Add(sizeName, size);

		using var con = await CreateConnection();
		using var rdr = await con.QueryMultipleAsync(query, p);

		var res = (await rdr.ReadAsync<T>()).ToArray();
		var total = await rdr.ReadSingleAsync<int>();

		var pages = (int)Math.Ceiling((double)total / size);
		return new(pages, total, res);
	}
}
