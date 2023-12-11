namespace CardboardBox.Database.Postgres.Standard;

/// <summary>
/// Helpful utilities for standard setups
/// </summary>
public static class Utilities
{
    private static readonly Random _rnd = new();

    /// <summary>
    /// Generates a random string of characters
    /// </summary>
    /// <param name="length">The number of characters to generate</param>
    /// <param name="chars">The characters to use</param>
    /// <returns>The random string of characters</returns>
    public static string RandomSuffix(int length = 10, string chars = "abcdefghijklmnopqrstuvwxyz")
    {
        return new string(
            Enumerable.Range(0, length)
                .Select(t => chars[_rnd.Next(chars.Length)])
                .ToArray());
    }
}
