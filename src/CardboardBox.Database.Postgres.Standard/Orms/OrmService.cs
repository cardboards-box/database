namespace CardboardBox.Database.Postgres.Standard.Orms;

/// <summary>
/// Wrap up service for ORMs
/// </summary>
public interface IOrmService
{
    /// <summary>
    /// The query service for the ORM
    /// </summary>
    IQueryService Query { get; }
    /// <summary>
    /// The SQL service for the ORM
    /// </summary>
    ISqlService Sql { get; }
    /// <summary>
    /// The optional interject service for the ORM
    /// </summary>
    IDbInterjectService? Interject { get; }

    /// <summary>
    /// The mapped queryable for the ORM
    /// </summary>
    /// <typeparam name="T">The type of table object</typeparam>
    /// <returns>The mapped queryable</returns>
    IOrmMapQueryable<T> For<T>() where T : DbObject;
}

internal class OrmService(
    IQueryService query,
    ISqlService sql,
    IDbInterjectService? interject = null) : IOrmService
{
    public IQueryService Query { get; } = query;
    public ISqlService Sql { get; } = sql;
    public IDbInterjectService? Interject { get; } = interject;

    public IOrmMapQueryable<T> For<T>() where T : DbObject => new OrmMap<T>(Query, Sql);
}
