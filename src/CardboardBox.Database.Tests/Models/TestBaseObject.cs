namespace CardboardBox.Database.Tests.Models;

public abstract class TestBaseObject
{
	[Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
	public long Id { get; set; }

	[Column(ExcludeInserts = true, ExcludeUpdates = true)]
	public DateTime CreatedAt { get; set; }

	[Column(ExcludeInserts = true, OverrideValue = "CURRENT_TIMESTAMP")]
	public DateTime UpdatedAt { get; set; }

	[Column]
	public DateTime? DeletedAt { get; set; }
}
