namespace CardboardBox.Database;

/// <summary>
/// Marks a property as a column to use within the table
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// The default property name to use for the <see cref="ForeignKeyProperty"/>
    /// </summary>
    public static string DEFAULT_FK_PROPERTY = "Id";

    /// <summary>
    /// The name of the column.
    /// </summary>
    /// <remarks>If not specified, the property name will be used</remarks>
    public string? Name { get; set; } = null;

    /// <summary>
    /// The value to use whenever this column is referenced in a query.
    /// </summary>
    /// <remarks>
    /// This is mostly for default columns to specific values **ALWAYS**,
    /// like ensuring that audit fields use CURRENT_TIMESTAMP for their initial and subsequent values.
    /// </remarks>
    public string? OverrideValue { get; set; } = null;

    /// <summary>
    /// Marks the column as being the primary key for the table.
    /// </summary>
    /// <remarks>
    /// This can only be specified once per table and can cause errors if multiple are specified.
    /// </remarks>
    public bool PrimaryKey { get; set; } = false;

    /// <summary>
    /// Whether or not to exclude this column when doing UPDATE queries.
    /// </summary>
    public bool ExcludeUpdates { get; set; } = false;

    /// <summary>
    /// Whether or not to exclude this column when doing INSERT queries
    /// </summary>
    public bool ExcludeInserts { get; set; } = false;

    /// <summary>
    /// Whether or not this column should be unique.
    /// </summary>
    /// <remarks>If multiple columns are specified, they will be assumed to be part of the same unique column set.</remarks>
    public bool Unique { get; set; } = false;

    /// <summary>
    /// Whether or not to ignore this column when creating queries.
    /// </summary>
    public bool Ignore { get; set; } = false;

    /// <summary>
    /// Indicates that this column is a foreign key to another table.
    /// </summary>
    public Type? ForeignKey { get; set; } = null;

    /// <summary>
    /// The name of the property that is referenced from the other table
    /// </summary>
    public string? ForeignKeyProperty { get; set; } = null;

    /// <summary>
    /// The data type of the column
    /// </summary>
    /// <remarks>Leaving null / empty will auto resolve the data type</remarks>
    public string? DataType { get; set; } = null;

    /// <summary>
    /// Marks a property as a column to use within the table
    /// </summary>
    public ColumnAttribute() { }

    /// <summary>
    /// Marks a property as a column to use within the table
    /// </summary>
    /// <param name="name">The name of the column.</param>
    public ColumnAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Marks a property as a column to use within the table
    /// </summary>
    /// <param name="foreignKey">The type reference to the foreign key type</param>
    /// <param name="foreignKeyProperty">The name of the property that is referenced from the other table</param>
    public ColumnAttribute(Type foreignKey, string? foreignKeyProperty = null)
    {
        ForeignKey = foreignKey;
        ForeignKeyProperty = foreignKeyProperty ?? DEFAULT_FK_PROPERTY;
    }

    /// <summary>
    /// Marks a property as a column to use within the table
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="foreignKey">The type reference to the foreign key type</param>
    /// <param name="foreignKeyProperty">The name of the property that is referenced from the other table</param>
    public ColumnAttribute(string name, Type foreignKey, string? foreignKeyProperty = null) : this(foreignKey, foreignKeyProperty)
    {
        Name = name;
    }
}
