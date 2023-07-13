namespace CardboardBox.Database.Generation;

/// <summary>
/// Determines how column and table names are escaped when generating queries
/// </summary>
public class QueryConfig
{
	/// <summary>
	/// The parameterization character
	/// </summary>
	public string ParameterCharacter { get; set; } = "@";

	/// <summary>
	/// The character to prefix to names to escape them
	/// </summary>
	public string EscapeStart { get; set; } = "[";

	/// <summary>
	/// The character to suffix to the names to escape them
	/// </summary>
	public string EscapeEnd { get; set; } = "]";

	/// <summary>
	/// Whether or not to escape the names
	/// </summary>
	public bool Escape { get; set; } = true;

	/// <summary>
	/// Determines how column and table names are escaped when generating queries
	/// </summary>
	public QueryConfig() { }

	/// <summary>
	/// Determines how column and table names are escaped when generating queries
	/// </summary>
	/// <param name="parameterCharacter">The parameterization character</param>
	/// <param name="escapeStart">The character to prefix to names to escape them</param>
	/// <param name="escapeEnd">The character to suffix to the names to escape them</param>
	/// <param name="escape">Whether or not to escape the names</param>
	public QueryConfig(string parameterCharacter, string escapeStart, string escapeEnd, bool escape)
	{
		ParameterCharacter = parameterCharacter;
		EscapeStart = escapeStart;
		EscapeEnd = escapeEnd;
		Escape = escape;
	}

}