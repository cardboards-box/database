namespace CardboardBox.Database.Postgres;

using Generation;

/// <summary>
/// A collection of useful query generation methods with a Postgresql flavour
/// </summary>
public class PostgresQueryGenerationService : QueryGenerationService
{
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
	public override string Paginate(TableConfig table, QueryConfig config, ColumnConfig[] where, string sortCol, bool sortAsc, string limitParam, string offsetParam)
	{
		const string QUERY = "SELECT * FROM {0}{1} " +
			"ORDER BY {2} {3} " +
			"LIMIT {4} " +
			"OFFSET {5}; " +
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
	public override string Upsert(TableConfig table, QueryConfig config, ColumnConfig[] conflicts, ColumnConfig[] inserts, ColumnConfig[] updates)
	{
		const string QUERY = "{0} ON CONFLICT ({1}) DO UPDATE SET {2}";
		var insert = Insert(table, config, inserts);
		var cols = Where(config, ", ", updates);
		var conf = string.Join(", ", conflicts.Select(t => t.Name));

		return string.Format(QUERY, insert, conf, cols);
	}


}
