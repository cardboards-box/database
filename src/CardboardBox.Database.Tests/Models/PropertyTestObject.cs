namespace CardboardBox.Database.Tests.Models;

[Table]
public class PropertyTestObject : TestBaseObject
{
	[Column(Unique = true)]
	public string UserName { get; set; } = string.Empty;

	[Column]
	public string FirstName { get; set; } = string.Empty;

	[Column]
	public string LastName { get; set; } = string.Empty;

	[Column("Email")]
	public string EmailAddress { get; set; } = string.Empty;
}
