namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a query generator service
/// </summary>
public interface IQueryGenerationService
{
	/// <summary>
	/// Escapes the given string based on the configuration specified
	/// </summary>
	/// <param name="value">The string to escape</param>
	/// <param name="config">The configuration for the escape</param>
	/// <returns>The escaped string</returns>
	string Escape(string value, QueryConfig config);

	/// <summary>
	/// Escapes the given table name based on the configuration specified
	/// </summary>
	/// <param name="table">The table name to escape</param>
	/// <param name="config">The configuration for the escape</param>
	/// <returns>The escaped table name</returns>
	string Escape(TableConfig table, QueryConfig config);

	/// <summary>
	/// Generates a SQL insert statement based on the given table, configuration, and columns
	/// </summary>
	/// <param name="table">The name of the table to insert into</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="columns">The columns to build into the insert query</param>
	/// <returns>The generated SQL INSERT query</returns>
	string Insert(TableConfig table, QueryConfig config, params ColumnConfig[] columns);

	/// <summary>
	/// Generates a SQL update statement based on the given table, configuration, where clause, and columns
	/// </summary>
	/// <param name="table">The name of the table to update</param>
	/// <param name="config">The configruation for the query generation</param>
	/// <param name="where">The where clause to apply to the update (minus the "WHERE")</param>
	/// <param name="values">The columns to update and their associated parameters / values</param>
	/// <returns>The generated SQL UPDATE query</returns>
	string Update(TableConfig table, QueryConfig config, string? where, params ColumnConfig[] values);

	/// <summary>
	/// Generates a SQL select * statement based on the given table, configuration, and where columns
	/// </summary>
	/// <param name="table">The name of the table to select from</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL SELECT query</returns>
	string Select(TableConfig table, QueryConfig config, params ColumnConfig[] where);

	/// <summary>
	/// Generates a SQL delete statement based on the given table, configruation and where columns
	/// </summary>
	/// <param name="table">The name of the table to delete from</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL DELTETE query</returns>
	string Delete(TableConfig table, QueryConfig config, params ColumnConfig[] where);

	/// <summary>
	/// Generates a where clause based on the given configuration, join operand, and where columns
	/// </summary>
	/// <param name="config">The configuration for the clause generation</param>
	/// <param name="join">The operand used to combine the where conditions (AND, OR, etc)</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL WHERE clause</returns>
	string Where(QueryConfig config, string join, params ColumnConfig[] where);

	/// <summary>
	/// Generates a multiple result set SQL query that paginates the results of the query.
	/// </summary>
	/// <param name="table">The table to query against</param>
	/// <param name="config">The configuration to use for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <param name="sortCol">The column to sort the results on</param>
	/// <param name="sortAsc">Whether to sort the sort column by ascending (true) or descending (false) results</param>
	/// <param name="limitParam">The name of the parameter to use for the return row count limits</param>
	/// <param name="offsetParam">The name of the parameter to use for the row offset</param>
	/// <returns>The generated SQL pagination query</returns>
	string Paginate(TableConfig table, QueryConfig config, ColumnConfig[] where, string sortCol, bool sortAsc, string limitParam, string offsetParam);

	/// <summary>
	/// Generates an upsert SQL query based on the given table and configuration
	/// </summary>
	/// <param name="table">The table to upsert into</param>
	/// <param name="config">The configuration to use for the query generation</param>
	/// <param name="conflicts">The columns that will cause the record to be updated instead of inserts. (These are normally a composite unique key)</param>
	/// <param name="inserts">The columns to insert if the record doesn't exist</param>
	/// <param name="updates">The columns to update if the record does exist</param>
	/// <returns>The generated SQL upsert query</returns>
	string Upsert(TableConfig table, QueryConfig config, ColumnConfig[] conflicts, ColumnConfig[] inserts, ColumnConfig[] updates);
}

/// <summary>
/// A collection of useful query generation methods.
/// </summary>
public class QueryGenerationService : IQueryGenerationService
{
	/// <summary>
	/// Escapes the given string based on the configuration specified
	/// </summary>
	/// <param name="value">The string to escape</param>
	/// <param name="config">The configuration for the escape</param>
	/// <returns>The escaped string</returns>
	public virtual string Escape(TableConfig value, QueryConfig config)
	{
		if (value.Prefixes == null || value.Prefixes.Length == 0)
			return Escape(value.Name, config);

		var parts = value.Prefixes.Append(value.Name);

		return string.Join(".", parts.Select(t => Escape(t, config)));
	}

	/// <summary>
	/// Escapes the given table name based on the configuration specified
	/// </summary>
	/// <param name="value">The table name to escape</param>
	/// <param name="config">The configuration for the escape</param>
	/// <returns>The escaped table name</returns>
	public virtual string Escape(string value, QueryConfig config)
	{
		if (string.IsNullOrEmpty(value) || !config.Escape) return value;

		return $"{config.EscapeStart}{value}{config.EscapeEnd}";
	}

	/// <summary>
	/// Generates a SQL insert statement based on the given table, configuration, and columns
	/// </summary>
	/// <param name="table">The name of the table to insert into</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="columns">The columns to build into the insert query</param>
	/// <returns>The generated SQL INSERT query</returns>
	public virtual string Insert(TableConfig table, QueryConfig config, params ColumnConfig[] columns)
	{
		if (columns == null || columns.Length == 0) throw new ArgumentNullException(nameof(columns));

		const string QUERY = "INSERT INTO {0} ({1}) VALUES ({2})";

		var cols = string.Join(", ", columns.Select(t => Escape(t.Name, config)));
		var pcols = string.Join(", ", columns.Select(t => t.Value ?? $"{config.ParameterCharacter}{t.ParamName ?? t.Name}"));

		return string.Format(QUERY, Escape(table, config), cols, pcols);
	}

	/// <summary>
	/// Generates a SQL update statement based on the given table, configuration, where clause, and columns
	/// </summary>
	/// <param name="table">The name of the table to update</param>
	/// <param name="config">The configruation for the query generation</param>
	/// <param name="where">The where clause to apply to the update (minus the "WHERE")</param>
	/// <param name="values">The columns to update and their associated parameters / values</param>
	/// <returns>The generated SQL UPDATE query</returns>
	public virtual string Update(TableConfig table, QueryConfig config, string? where, params ColumnConfig[] values)
	{
		if (values == null || values.Length == 0) throw new ArgumentNullException(nameof(values));

		const string QUERY = "UPDATE {0} SET {1}{2}";

		var cols = Where(config, ", ", values);

		return string.Format(QUERY, Escape(table, config), cols, string.IsNullOrEmpty(where) ? "" : " WHERE " + where);
	}

	/// <summary>
	/// Generates a SQL select * statement based on the given table, configuration, and where columns
	/// </summary>
	/// <param name="table">The name of the table to select from</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL SELECT query</returns>
	public virtual string Select(TableConfig table, QueryConfig config, params ColumnConfig[] where)
	{
		if (where == null || where.Length == 0) return $"SELECT * FROM {Escape(table, config)}";

		const string QUERY = "SELECT * FROM {0} WHERE {1}";
		var cols = Where(config, " AND ", where);
		return string.Format(QUERY, Escape(table, config), cols);
	}

	/// <summary>
	/// Generates a SQL delete statement based on the given table, configruation and where columns
	/// </summary>
	/// <param name="table">The name of the table to delete from</param>
	/// <param name="config">The configuration for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL DELTETE query</returns>
	public virtual string Delete(TableConfig table, QueryConfig config, params ColumnConfig[] where)
	{
		if (where == null || where.Length == 0) return $"DELETE FROM {Escape(table, config)}";

		const string QUERY = "DELETE FROM {0} WHERE {1}";
		var cols = Where(config, " AND ", where);
		return string.Format(QUERY, Escape(table, config), cols);
	}

	/// <summary>
	/// Generates a where clause based on the given configuration, join operand, and where columns
	/// </summary>
	/// <param name="config">The configuration for the clause generation</param>
	/// <param name="join">The operand used to combine the where conditions (AND, OR, etc)</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <returns>The generated SQL WHERE clause</returns>
	public virtual string Where(QueryConfig config, string join, params ColumnConfig[] where)
	{
		return string.Join(join,
			where.Select(t => $"{Escape(t.Name, config)} {t.Operand ?? "="} {t.Value ?? config.ParameterCharacter + (t.ParamName ?? t.Name)}"));
	}

	/// <summary>
	/// Generates a multiple result set SQL query that paginates the results of the query.
	/// </summary>
	/// <param name="table">The table to query against</param>
	/// <param name="config">The configuration to use for the query generation</param>
	/// <param name="where">The where clause columns and their associated parameters / values</param>
	/// <param name="sortCol">The column to sort the results on</param>
	/// <param name="sortAsc">Whether to sort the sort column by ascending (true) or descending (false) results</param>
	/// <param name="limitParam">The name of the parameter to use for the return row count limits</param>
	/// <param name="offsetParam">The name of the parameter to use for the row offset</param>
	/// <returns>The generated SQL pagination query</returns>
	public virtual string Paginate(TableConfig table, QueryConfig config, ColumnConfig[] where, string sortCol, bool sortAsc, string limitParam, string offsetParam)
	{
		const string QUERY = "SELECT * FROM {0}{1} " +
			"ORDER BY {2} {3} " +
			"OFFSET {5} ROWS " +
			"FETCH NEXT {4} ROWS ONLY; " +
			"SELECT COUNT(*) FROM {0}{1};";

		var whereClause = where.Length == 0 ? "" : " WHERE " + Where(config, " AND ", where);

		return string.Format(QUERY,
			Escape(table, config),
			whereClause,
			Escape(sortCol, config),
			sortAsc ? "ASC" : "DESC",
			config.ParameterCharacter + limitParam,
			config.ParameterCharacter + offsetParam);
	}

	/// <summary>
	/// Generates an upsert SQL query based on the given table and configuration
	/// </summary>
	/// <param name="table">The table to upsert into</param>
	/// <param name="config">The configuration to use for the query generation</param>
	/// <param name="conflicts">The columns that will cause the record to be updated instead of inserts. (These are normally a composite unique key)</param>
	/// <param name="inserts">The columns to insert if the record doesn't exist</param>
	/// <param name="updates">The columns to update if the record does exist</param>
	/// <returns>The generated SQL upsert query</returns>
	public virtual string Upsert(TableConfig table, QueryConfig config, ColumnConfig[] conflicts, ColumnConfig[] inserts, ColumnConfig[] updates)
	{
		const string QUERY = @"{0}; IF (@@ROWCOUNT = 0) {1};";

		var where = Where(config, " AND ", conflicts);
		var insert = Insert(table, config, inserts);
		var update = Update(table, config, where, updates);

		return string.Format(QUERY, update, insert);
	}
}