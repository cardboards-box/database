namespace CardboardBox.Database;

/// <summary>
/// Indicates that a property is a foreign key to another entity
/// </summary>
/// <param name="type">The type of entity being referenced</param>
/// <param name="property">The name of the property to reference on the entity (defaults to Id)</param>
/// <param name="ignore">Whether or not to ignore this entity when resolving all relationships</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FkAttribute(
    Type type,
    string property = "Id",
    bool ignore = false) : Attribute
{
    /// <summary>
    /// The type of entity being referenced
    /// </summary>
    public Type Type { get; } = type;

    /// <summary>
    /// The name of the property to reference on the entity (defaults to Id)
    /// </summary>
    public string Property { get; } = property;

    /// <summary>
    /// Whether or not to ignore this entity when resolving all relationships
    /// </summary>
    public bool IgnoreInRelationship { get; } = ignore;
}

/// <summary>
/// Indicates that a property is a foreign key to another entity
/// </summary>
/// <typeparam name="T">The type of entity being referenced</typeparam>
/// <param name="property">The name of the property to reference on the entity (defaults to Id)</param>
/// <param name="ignore">Whether or not to ignore this entity when resolving all relationships</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FkAttribute<T>(
    string property = "Id",
    bool ignore = false) : FkAttribute(typeof(T), property, ignore)
{ }

