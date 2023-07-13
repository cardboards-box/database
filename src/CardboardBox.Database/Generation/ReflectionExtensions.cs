namespace CardboardBox.Database.Generation;

/// <summary>
/// A collection of useful extension methods for reflection things
/// </summary>
public static class ReflectionExtensions
{
	/// <summary>
	/// Gets the <see cref="PropertyInfo"/> data from a given lambda expression
	/// </summary>
	/// <typeparam name="TSource">The source type</typeparam>
	/// <typeparam name="TProp">The property type</typeparam>
	/// <param name="propertyLambda">The lambda expression to resolve the property</param>
	/// <returns>The property info for the given lambda expression</returns>
	/// <exception cref="ArgumentNullException">Thrown when the inputted lambda expression is null</exception>
	/// <exception cref="ArgumentException">Thrown when the lambda expression resolves to a method, field, or is not from the source type (How did you even...?)</exception>
	public static PropertyInfo GetPropertyInfo<TSource, TProp>(this Expression<Func<TSource, TProp>>? propertyLambda)
	{
		if (propertyLambda == null) throw new ArgumentNullException(nameof(propertyLambda));

		var type = typeof(TSource);

		if (propertyLambda.Body is not MemberExpression member)
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a method, not a property.",
				propertyLambda.ToString()));

		var propInfo = member.Member as PropertyInfo;
		if (propInfo == null)
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a field, not a property.",
				propertyLambda.ToString()));

		if (propInfo.ReflectedType != null &&
			type != propInfo.ReflectedType &&
			!type.IsSubclassOf(propInfo.ReflectedType))
			throw new ArgumentException(string.Format(
				"Expression '{0}' refers to a property that is not from type {1}.",
				propertyLambda.ToString(),
				type));

		return propInfo;
	}
}
