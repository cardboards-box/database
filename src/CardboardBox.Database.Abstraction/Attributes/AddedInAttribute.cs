namespace CardboardBox.Database;

/// <summary>
/// Indicates that the column was added in a later version of the application
/// </summary>
/// <param name="version">The version that the column was added in</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AddedInAttribute(int version) : Attribute
{
    /// <summary>
    /// The version that the column was added in
    /// </summary>
    public int Version { get; } = version;
}
