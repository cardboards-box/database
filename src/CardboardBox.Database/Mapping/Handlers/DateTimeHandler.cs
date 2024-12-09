namespace CardboardBox.Database.Mapping;

/// <summary>
/// Handles mapping <see cref="DateTime"/> results from string results.
/// This a polyfill to add proper <see cref="DateTime"/> handling to engines like SQLite
/// </summary>
public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
	internal static readonly DateTime UnixOrigin = new(1970, 1, 1, 0, 0, 0, 0);

	/// <summary>
	/// Parses the input value into a proper <see cref="DateTime"/>
	/// </summary>
	/// <param name="value">The value to parse</param>
	/// <returns>The parsed <see cref="DateTime"/></returns>
	public override DateTime Parse(object value)
	{
		return value switch
		{
			DateTime time => time,
			string str => DateTime.TryParse(str, out var res) ? res : DateTime.MinValue,
			long val => UnixOrigin.AddSeconds(val),
			int val => UnixOrigin.AddSeconds(val),
			float val => UnixOrigin.AddSeconds((long)val),
			_ => DateTime.MinValue
		};
	}

	/// <summary>
	/// Sets the database parameter value to the proper representation for a <see cref="DateTime"/>
	/// </summary>
	/// <param name="parameter">The database parameter to set</param>
	/// <param name="value">The value to set it to</param>
	public override void SetValue(IDbDataParameter parameter, DateTime value)
	{
		parameter.Value = value.ToString("yyyy-MM-dd HH:mm:ss.fff");
	}
}

/// <summary>
/// Handles mapping nullable <see cref="DateTime"/> results from integer or string results.
/// This a polyfill to add proper nullable <see cref="DateTime"/> handling to engines like SQLite
/// </summary>
public class NullableDateTimeHandler : SqlMapper.TypeHandler<DateTime?>
{
	/// <summary>
	/// Parses the input value into a proper nullable <see cref="DateTime"/>
	/// </summary>
	/// <param name="value">The value to parse</param>
	/// <returns>The parsed nullable <see cref="DateTime"/></returns>
	public override DateTime? Parse(object value)
	{
		if (value == null) return null;

		return value switch
		{
			DateTime time => time,
			string str => DateTime.TryParse(str, out var res) ? res : null,
			long val => DateTimeHandler.UnixOrigin.AddSeconds(val),
			int val => DateTimeHandler.UnixOrigin.AddSeconds(val),
			float val => DateTimeHandler.UnixOrigin.AddSeconds((long)val),
			_ => null
		};
	}

	/// <summary>
	/// Sets the database parameter value to the proper representation for a nullable <see cref="DateTime"/>
	/// </summary>
	/// <param name="parameter">The database parameter to set</param>
	/// <param name="value">The value to set it to</param>
	public override void SetValue(IDbDataParameter parameter, DateTime? value)
	{
		parameter.Value = value?.ToString("yyyy-MM-dd HH:mm:ss.fff");
	}
}