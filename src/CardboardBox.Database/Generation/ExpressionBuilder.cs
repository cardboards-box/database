namespace CardboardBox.Database.Generation;

/// <summary>
/// Represents an expression builder that can only resolve properties (No complex querying)
/// </summary>
/// <typeparam name="T">The type of class the properties are coming from</typeparam>
public interface IPropertyBinder<T>
{
	/// <summary>
	/// Appends the target property to the expression builder
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	IPropertyBinder<T> Prop<TProp>(Expression<Func<T, TProp>> property);
}

/// <summary>
/// Represents an expression builder that can resolve and query properties
/// </summary>
/// <typeparam name="T">The type of class the properties are coming from</typeparam>
public interface IExpressionBuilder<T> : IPropertyBinder<T>
{
	/// <summary>
	/// Appends the target property to the expression builder
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	IExpressionBuilder<T> With<TProp>(Expression<Func<T, TProp>> property);

	/// <summary>
	/// Appens the target property to the expression builder with the complex query options specified
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <param name="value">The optional value to override the property value with</param>
	/// <param name="operand">The operand to use when querying this property</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	IExpressionBuilder<T> Exp<TProp>(Expression<Func<T, TProp>> property, string? value = null, string? operand = null);

	/// <summary>
	/// Appends the target property to the expression builder with a "IS NULL" query
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	IExpressionBuilder<T> Null<TProp>(Expression<Func<T, TProp>> property);

	/// <summary>
	/// Appends the target property to the expression builder with a "IS NOT NULL" query
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	IExpressionBuilder<T> NotNull<TProp>(Expression<Func<T, TProp>> property);
}

/// <summary>
/// An builder that allows for building queries from lambda expressions
/// </summary>
/// <typeparam name="T">The type of class the properties are coming from</typeparam>
public class ExpressionBuilder<T> : IExpressionBuilder<T>, IPropertyBinder<T>
{
	/// <summary>
	/// The target type
	/// </summary>
	public ReflectedType Type { get; }

	/// <summary>
	/// The outputed expressions
	/// </summary>
	public List<PropValue> Properties { get; } = new();

	/// <summary>
	/// An builder that allows for building queries from lambda expressions
	/// </summary>
	/// <param name="type">The target type</param>
	public ExpressionBuilder(ReflectedType type)
	{
		Type = type;
	}

	/// <summary>
	/// Appends the target property to the expression builder
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	public IExpressionBuilder<T> With<TProp>(Expression<Func<T, TProp>> property) => Exp(property);

	/// <summary>
	/// Appens the target property to the expression builder with the complex query options specified
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <param name="value">The optional value to override the property value with</param>
	/// <param name="operand">The operand to use when querying this property</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	public IExpressionBuilder<T> Exp<TProp>(Expression<Func<T, TProp>> property, string? value = null, string? operand = null)
	{
		var prop = property.GetPropertyInfo();

		if (!Type.Properties.TryGetValue(prop.Name, out var reflected))
			throw new ArgumentException($"Invalid property detected, \"{prop.Name}\"! Is it ignored? ", nameof(property));

		Properties.Add(new(reflected, value, operand));
		return this;
	}

	/// <summary>
	/// Appends the target property to the expression builder with a "IS NULL" query
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	public IExpressionBuilder<T> Null<TProp>(Expression<Func<T, TProp>> property) => Exp(property, "NULL", "IS");

	/// <summary>
	/// Appends the target property to the expression builder with a "IS NOT NULL" query
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	public IExpressionBuilder<T> NotNull<TProp>(Expression<Func<T, TProp>> property) => Exp(property, "NULL", "IS NOT");

	/// <summary>
	/// Appends the target property to the expression builder
	/// </summary>
	/// <typeparam name="TProp">The type of property that is being resolved</typeparam>
	/// <param name="property">The property that is being resolved</param>
	/// <returns>The current instance of the expression builder for chaining</returns>
	public IPropertyBinder<T> Prop<TProp>(Expression<Func<T, TProp>> property) => Exp(property);
}

