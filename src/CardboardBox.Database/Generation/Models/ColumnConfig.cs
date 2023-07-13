namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a column parsed from either an <see cref="IExpressionBuilder{T}"/> or a <see cref="ColumnAttribute"/>
/// </summary>
/// <param name="Name">The name of the property or the <see cref="ColumnAttribute.Name"/></param>
/// <param name="ParamName">The name to use for any parameterized version of this property. This is normally the C# Property Name</param>
/// <param name="Value">The overridable value for the property. This is used for building complex expressions (like IS NULL or IS NOT NULL)</param>
/// <param name="Operand">The operand to use when asserting this property (defaults to '=' but can be 'IS' or 'IS NOT'</param>
public record class ColumnConfig(
	string Name, 
	string? ParamName, 
	string? Value, 
	string? Operand);