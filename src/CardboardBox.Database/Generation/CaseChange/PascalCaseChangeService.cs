namespace CardboardBox.Database.Generation.CaseChange;

/// <summary>
/// Converts the inputted string to pascalCase
/// </summary>
public class PascalCaseChangeService : ICaseChangeService
{
	/// <summary>
	/// Converts the given string to pascalCase
	/// </summary>
	/// <param name="text">The input string</param>
	/// <returns>The input string converted to pascalCase</returns>
	public string ChangeCase(string text)
	{
		var chars = text.ToCharArray();
		chars[0] = char.ToLowerInvariant(chars[0]);
		return new string(chars);
	}
}
