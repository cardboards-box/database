namespace CardboardBox.Database.Generation.CaseChange;

/// <summary>
/// Converts strings to the camel_case
/// </summary>
public class CamelCaseChangeService : ICaseChangeService
{
	/// <summary>
	/// Converts the given input to camel_case
	/// </summary>
	/// <param name="text">The input string</param>
	/// <returns>The input string in camel_case</returns>
	/// <exception cref="ArgumentNullException">Thrown if the input text is null or empty</exception>
	public string ChangeCase(string text)
	{
		if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
		if (text.Length < 2) return text;

		var sb = new StringBuilder();
		sb.Append(char.ToLowerInvariant(text[0]));
		for (int i = 1; i < text.Length; ++i)
		{
			char c = text[i];
			if (!char.IsUpper(c))
			{
				sb.Append(c);
				continue;
			}

			sb.Append('_');
			sb.Append(char.ToLowerInvariant(c));
		}

		return sb.ToString();
	}
}
