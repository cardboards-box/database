using System.Text.Json.Serialization;

namespace CardboardBox.Database;

/// <summary>
/// Represents a paginated result from the database
/// </summary>
/// <typeparam name="T">The model type to return</typeparam>
public class PaginatedResult<T>
{
	/// <summary>
	/// The number of available pages of results
	/// </summary>
	[JsonPropertyName("pages")]
	public int Pages { get; set; }

	/// <summary>
	/// The total number of results
	/// </summary>
	[JsonPropertyName("count")]
	public int Count { get; set; }

	/// <summary>
	/// The results for this page
	/// </summary>
	[JsonPropertyName("results")]
	public T[] Results { get; set; } = Array.Empty<T>();

	/// <summary>
	/// Represents a paginated result from the database
	/// </summary>
	public PaginatedResult() { }

	/// <summary>
	/// Represents a paginated result from the database
	/// </summary>
	/// <param name="pages">The number of available pages of results</param>
	/// <param name="count">The total number of results</param>
	/// <param name="results">The results for this page</param>
	public PaginatedResult(int pages, int count, T[] results)
	{
		Pages = pages;
		Count = count;
		Results = results;
	}

	/// <summary>
	/// Represents a paginated result from the database
	/// </summary>
	/// <param name="pages">The number of available pages of results</param>
	/// <param name="count">The total number of results</param>
	/// <param name="results">The results for this page</param>
	public void Deconstruct(out int pages, out int count, out T[] results)
	{
		pages = Pages;
		count = Count;
		results = Results;
	}
}
