namespace CardboardBox.Database.Generation.CaseChange;

/// <summary>
/// Doesn't do anything to the case of the input string
/// </summary>
public class NullCaseChangeService : ICaseChangeService
{
	/// <summary>
	/// Just returns the same input back
	/// </summary>
	/// <param name="input">The input string</param>
	/// <returns>The input string with 0 changes</returns>
	public string ChangeCase(string input) => input;
}
