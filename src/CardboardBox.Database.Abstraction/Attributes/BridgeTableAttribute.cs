namespace CardboardBox.Database;

/// <summary>
/// Indicates that this table serves as a bridge between two other tables
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class BridgeTableAttribute(
    Type parent,
    Type child) : Attribute
{
    /// <summary>
    /// The type of the class the represents the entity that will have the least number of records
    /// </summary>
    public Type Parent { get; } = parent;

    /// <summary>
    /// The type of the class that represents the entity that will have the most number of records
    /// </summary>
    public Type Child { get; } = child;

    /// <summary>
    /// Whether or not to include the related entity when resolving the child type
    /// </summary>
    public bool IncludeInChildOrm { get; set; } = true;

    /// <summary>
    /// Whether or not to include the related entity when resolving the parent type
    /// </summary>
    public bool IncludeInParentOrm { get; set; } = true;
}

/// <summary>
/// Indicates that this table serves as a bridge between two other tables
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class BridgeTableAttribute<TParent, TChild>() : BridgeTableAttribute(typeof(TParent), typeof(TChild))
    where TParent : class, IDbTable
    where TChild : class, IDbTable
{

}
