namespace CardboardBox.Database.Generation;

/// <summary>
/// An overrideable service that allows for changing the case (Snake, Pascal, None) of columns and database fields
/// </summary>
public interface ICaseChangeService
{
	/// <summary>
	/// Changes the given input string to the case convention
	/// </summary>
	/// <param name="input">The string to change</param>
	/// <returns>The string in the correct case</returns>
	string ChangeCase(string input);
}
