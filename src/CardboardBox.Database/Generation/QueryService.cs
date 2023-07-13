namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a service that generates SQL queries based on the given POCOs
/// </summary>
public interface IQueryService
{
	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given generic
	/// </summary>
	/// <typeparam name="T">The type of class to get</typeparam>
	/// <returns>The <see cref="ReflectedType"/> for the given generic</returns>
	ReflectedType Type<T>();

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given expression builder
	/// </summary>
	/// <typeparam name="T">The type of POCO we're dealing with</typeparam>
	/// <param name="bob">The expression builder</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given expression builder</returns>
	ColumnConfig[] From<T>(ExpressionBuilder<T> bob, bool @override = true);

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given property list
	/// </summary>
	/// <param name="props">The properties we're dealing with</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given property list</returns>
	ColumnConfig[] From(IEnumerable<PropValue> props, bool @override = true);

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given property list
	/// </summary>
	/// <param name="props">The properties we're dealing with</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given property list</returns>
	ColumnConfig[] From(IEnumerable<ReflectedProperty> props, bool @override = true);

	/// <summary>
	/// Gets the <see cref="ColumnConfig"/> for the given property expression
	/// </summary>
	/// <typeparam name="T1">The type of POCO we're dealing with</typeparam>
	/// <typeparam name="T2">The return type of the property</typeparam>
	/// <param name="type">The <see cref="ReflectedType"/> of the POCO we're dealing with</param>
	/// <param name="property">The lambda expression to fetch the property</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/> for the given property expression</returns>
	/// <exception cref="ArgumentException">Thrown if the property expression resolves to an invalid column</exception>
	ColumnConfig From<T1, T2>(ReflectedType type, Expression<Func<T1, T2>> property, bool @override = true);

	/// <summary>
	/// Generates a SQL query based on the given type and conditions
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="where">The conditions to select using</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL query</returns>
	string Select<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null);

	/// <summary>
	/// Generates a SQL query that fetches a record via it's primary key
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL query</returns>
	string Fetch<T>(QueryConfig? config = null);

	/// <summary>
	/// Generates an INSERT SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to insert</typeparam>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL INSERT query</returns>
	string Insert<T>(QueryConfig? config = null);

	/// <summary>
	/// Generates an UPDATE SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to update</typeparam>
	/// <param name="where">The conditions to update against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL UPDATE query</returns>
	string Update<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null);

	/// <summary>
	/// Generates an UPDATE SQL query for the given type that only updates the given properties
	/// </summary>
	/// <typeparam name="T">The type of object to update</typeparam>
	/// <param name="set">The columns that should be updated</param>
	/// <param name="where">The conditions to update against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL UPDATE query</returns>
	/// <exception cref="ArgumentException">Thrown if the where query cannot be found or if no set properties are specified</exception>
	string UpdateOnly<T>(Action<IPropertyBinder<T>> set, Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null);

	/// <summary>
	/// Generates a DELETE SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to delete</typeparam>
	/// <param name="where">The conditions to delete against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL DELETE query</returns>
	string Delete<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null);

	/// <summary>
	/// Generates a paginated select query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <typeparam name="TSort">The return type for the sort column</typeparam>
	/// <param name="sortBy">The property to sort the results by</param>
	/// <param name="sortAsc">Whether or not to sort the query in ascending (true) or descending (false) order</param>
	/// <param name="where">The conditions to query against</param>
	/// <param name="config">The configuration for query generation</param>
	/// <param name="limitName">The name of the parameter to use for return row count limit</param>
	/// <param name="offsetName">The name of the parameter to use for the row offset</param>
	/// <returns>The generated SQL pagination query</returns>
	string Paginate<T, TSort>(
		Expression<Func<T, TSort>> sortBy, bool sortAsc = true,
		Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null,
		string limitName = "limit", string offsetName = "offset");

	/// <summary>
	/// Generates an UPSERT SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="conflicts">The columns that make up the composite unique key.</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL upsert query</returns>
	string Upsert<T>(Action<IPropertyBinder<T>>? conflicts = null, QueryConfig? config = null);
}

/// <summary>
/// A service that generates SQL queries based on given POCO types.
/// </summary>
public class QueryService : IQueryService
{
	private readonly IReflectedService _reflected;
	private readonly ICaseChangeService _caseChange;
	private readonly IQueryGenerationService _raw;
	private readonly IQueryConfigProvider _config;

	/// <summary>
	/// Dependency Injection Constructor
	/// </summary>
	/// <param name="reflected">The service that handles caching <see cref="ReflectedType"/></param>
	/// <param name="caseChange">The service that handles correcting the case of database fields</param>
	/// <param name="raw">The service that handles generating the raw SQL queries</param>
	/// <param name="config">The service that handles providing the default <see cref="QueryConfig"/>s for query generation</param>
	public QueryService(
		IReflectedService reflected,
		ICaseChangeService caseChange,
		IQueryGenerationService raw,
		IQueryConfigProvider config)
	{
		_reflected = reflected;
		_caseChange = caseChange;
		_raw = raw;
		_config = config;
	}

	/// <summary>
	/// Gets the <see cref="ReflectedType"/> for the given generic
	/// </summary>
	/// <typeparam name="T">The type of class to get</typeparam>
	/// <returns>The <see cref="ReflectedType"/> for the given generic</returns>
	public ReflectedType Type<T>() => _reflected.GetType<T>();

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given expression builder
	/// </summary>
	/// <typeparam name="T">The type of POCO we're dealing with</typeparam>
	/// <param name="bob">The expression builder</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given expression builder</returns>
	public ColumnConfig[] From<T>(ExpressionBuilder<T> bob, bool @override = true)
	{
		return From(bob.Properties, @override);
	}

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given property list
	/// </summary>
	/// <param name="props">The properties we're dealing with</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given property list</returns>
	public ColumnConfig[] From(IEnumerable<PropValue> props, bool @override = true)
	{
		return props
			.Select(exp =>
			{
				var name = _caseChange.ChangeCase(exp.Prop.Name);
				var value = exp.Value ?? (@override ? exp.Prop.Column?.OverrideValue : null);

				return new ColumnConfig(name, exp.Prop.Property.Name, value, exp.Operand);
			}).ToArray();
	}

	/// <summary>
	/// Generates the <see cref="ColumnConfig"/>s for the given property list
	/// </summary>
	/// <param name="props">The properties we're dealing with</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/>s for the given property list</returns>
	public ColumnConfig[] From(IEnumerable<ReflectedProperty> props, bool @override = true)
	{
		return props
			.Select(exp =>
			{
				var name = _caseChange.ChangeCase(exp.Name);
				var value = @override ? exp.Column?.OverrideValue : null;

				return new ColumnConfig(name, exp.Property.Name, value, null);
			}).ToArray();
	}

	/// <summary>
	/// Gets the <see cref="ColumnConfig"/> for the given property expression
	/// </summary>
	/// <typeparam name="T1">The type of POCO we're dealing with</typeparam>
	/// <typeparam name="T2">The return type of the property</typeparam>
	/// <param name="type">The <see cref="ReflectedType"/> of the POCO we're dealing with</param>
	/// <param name="property">The lambda expression to fetch the property</param>
	/// <param name="override">Whether or not to use the override values (true) or parameter names (false) in the column configs</param>
	/// <returns>The <see cref="ColumnConfig"/> for the given property expression</returns>
	/// <exception cref="ArgumentException">Thrown if the property expression resolves to an invalid column</exception>
	public ColumnConfig From<T1, T2>(ReflectedType type, Expression<Func<T1, T2>> property, bool @override = true)
	{
		var prop = property.GetPropertyInfo();
		if (!type.Properties.TryGetValue(prop.Name, out var exp))
			throw new ArgumentException($"Invalid property detected, \"{prop.Name}\"! Is it ignored? ", nameof(property));

		var name = _caseChange.ChangeCase(exp.Name);
		var value = @override ? exp.Column?.OverrideValue : null;

		return new ColumnConfig(name, exp.Property.Name, value, null);
	}

	/// <summary>
	/// Generates a SQL query based on the given type and conditions
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="where">The conditions to select using</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL query</returns>
	public string Select<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();
		var bob = new ExpressionBuilder<T>(map);

		where?.Invoke(bob);

		return _raw.Select(map.Name, config, From(bob, false));
	}

	/// <summary>
	/// Generates a SQL query that fetches a record via it's primary key
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL query</returns>
	public string Fetch<T>(QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();

		var pk = map.Properties
			.Values
			.Where(t => t.Column?.PrimaryKey ?? false)
			.ToArray();

		if (!pk.Any()) throw new ArgumentException("No primary key found for type: " + map.Name);

		return _raw.Select(map.Name, config, From(pk, false));
	}

	/// <summary>
	/// Generates an INSERT SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to insert</typeparam>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL INSERT query</returns>
	public string Insert<T>(QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();
		var cols = map.Properties
			.Values
			.Where(t => !(t.Column?.ExcludeInserts ?? false));

		return _raw.Insert(map.Name, config, From(cols));
	}

	/// <summary>
	/// Generates an UPDATE SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to update</typeparam>
	/// <param name="where">The conditions to update against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL UPDATE query</returns>
	public string Update<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();
		var cols = map.Properties
			.Values
			.Where(t => !(t.Column?.ExcludeUpdates ?? false));

		var bob = new ExpressionBuilder<T>(map);
		where?.Invoke(bob);

		if (where != null && bob.Properties.Count > 0)
		{
			var w = _raw.Where(config, " AND ", From(bob, false));
			return _raw.Update(map.Name, config, w, From(cols));
		}

		var pk = map.Properties
			.Values
			.Where(t => t.Column?.PrimaryKey ?? false)
			.ToArray();

		if (!pk.Any()) throw new ArgumentException("No primary key found for type: " + map.Name);
		var wp = _raw.Where(config, " AND ", From(pk, false));
		return _raw.Update(map.Name, config, wp, From(cols));
	}

	/// <summary>
	/// Generates an UPDATE SQL query for the given type that only updates the given properties
	/// </summary>
	/// <typeparam name="T">The type of object to update</typeparam>
	/// <param name="set">The columns that should be updated</param>
	/// <param name="where">The conditions to update against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL UPDATE query</returns>
	/// <exception cref="ArgumentException">Thrown if the where query cannot be found or if no set properties are specified</exception>
	public string UpdateOnly<T>(Action<IPropertyBinder<T>> set, Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();

		var whereBob = new ExpressionBuilder<T>(map);
		where?.Invoke(whereBob);

		string? wp = null;

		if (where != null && whereBob.Properties.Count > 0)
			wp = _raw.Where(config, " AND ", From(whereBob, false));
		else
		{
			var pk = map.Properties
				.Values
				.Where(t => t.Column?.PrimaryKey ?? false)
				.ToArray();

			if (!pk.Any()) throw new ArgumentException("No primary key found for type: " + map.Name);

			wp = _raw.Where(config, " AND ", From(pk, false));
		}

		var setBob = new ExpressionBuilder<T>(map);
		set?.Invoke(setBob);

		if (setBob.Properties.Count == 0) throw new ArgumentException("No set properties found");

		var cols = From(setBob, true);
		return _raw.Update(map.Name, config, wp, cols);
	}

	/// <summary>
	/// Generates a DELETE SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to delete</typeparam>
	/// <param name="where">The conditions to delete against</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL DELETE query</returns>
	public string Delete<T>(Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();
		var bob = new ExpressionBuilder<T>(map);
		where?.Invoke(bob);

		if (where != null && bob.Properties.Count > 0)
			return _raw.Delete(map.Name, config, From(bob, false));

		var pk = map.Properties
			.Values
			.Where(t => t.Column?.PrimaryKey ?? false)
			.ToArray();

		if (!pk.Any()) throw new ArgumentException("No primary key found for type: " + map.Name);
		return _raw.Delete(map.Name, config, From(pk, false));
	}

	/// <summary>
	/// Generates a paginated select query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <typeparam name="TSort">The return type for the sort column</typeparam>
	/// <param name="sortBy">The property to sort the results by</param>
	/// <param name="sortAsc">Whether or not to sort the query in ascending (true) or descending (false) order</param>
	/// <param name="where">The conditions to query against</param>
	/// <param name="config">The configuration for query generation</param>
	/// <param name="limitName">The name of the parameter to use for return row count limit</param>
	/// <param name="offsetName">The name of the parameter to use for the row offset</param>
	/// <returns>The generated SQL pagination query</returns>
	public string Paginate<T, TSort>(
		Expression<Func<T, TSort>> sortBy, bool sortAsc = true,
		Action<IExpressionBuilder<T>>? where = null, QueryConfig? config = null,
		string limitName = "limit", string offsetName = "offset")
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();
		var bob = new ExpressionBuilder<T>(map);
		where?.Invoke(bob);

		var sort = From(map, sortBy).Name;
		return _raw.Paginate(map.Name, config, From(bob, false), sort, sortAsc, limitName, offsetName);
	}

	/// <summary>
	/// Generates an UPSERT SQL query for the given type
	/// </summary>
	/// <typeparam name="T">The type of object to query against</typeparam>
	/// <param name="conflicts">The columns that make up the composite unique key.</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <returns>The generated SQL upsert query</returns>
	public string Upsert<T>(Action<IPropertyBinder<T>>? conflicts = null, QueryConfig? config = null)
	{
		config ??= _config.GetQueryConfig();
		var map = _reflected.GetType<T>();

		var updates = map.Properties
			.Values
			.Where(t => !(t.Column?.ExcludeUpdates ?? false));
		var inserts = map.Properties
			.Values
			.Where(t => !(t.Column?.ExcludeInserts ?? false));

		var bob = new ExpressionBuilder<T>(map);
		conflicts?.Invoke(bob);

		if (conflicts != null && bob.Properties.Count > 0)
			return _raw.Upsert(map.Name, config, From(bob, false), From(inserts), From(updates));

		var uq = map.Properties
			.Values
			.Where(t => t.Column?.Unique ?? false)
			.ToArray();

		if (!uq.Any()) throw new ArgumentException("No primary key found for type: " + map.Name);

		return _raw.Upsert(map.Name, config, From(uq, false), From(inserts), From(updates));
	}
}
