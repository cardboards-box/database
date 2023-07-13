namespace CardboardBox.Database.Tests.Models;

[Table("AreYou", "HelloWorld", "How")]
public class ModifiedTableObject : TestBaseObject
{
	[Column("Msg")]
	public string Message { get; set; } = string.Empty;
}