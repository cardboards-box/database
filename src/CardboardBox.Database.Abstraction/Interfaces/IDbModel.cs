namespace CardboardBox.Database;

/// <summary>
/// Represents a model that is stored in the database
/// </summary>
public interface IDbModel { }

/// <summary>
/// Represents a table in the database
/// </summary>
public interface IDbTable : IDbModel { }

/// <summary>
/// Represents a table type in the database
/// </summary>
public interface IDbType : IDbModel { }

/// <summary>
/// Represents a table whose data is cached locally
/// </summary>
public interface IDbCacheTable : IDbTable { }
