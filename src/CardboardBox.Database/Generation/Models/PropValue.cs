namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents a property resolved via an <see cref="ExpressionBuilder{T}"/>
/// </summary>
/// <param name="Prop">The <see cref="ReflectedProperty"/> for this property</param>
/// <param name="Value">The optional override value for this property. See <see cref="ColumnConfig.Value"/></param>
/// <param name="Operand">The optional operand for this property. See <see cref="ColumnConfig.Operand"/></param>
public record class PropValue(ReflectedProperty Prop, string? Value, string? Operand);
