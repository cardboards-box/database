using CardboardBox.Database;
using CardboardBox.Database.Generation;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp;

[Table("users")]
public class TestUserSqlite
{
	[Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
	public int Id { get; set; }

	[Column(Unique = true)]
	public string UserName { get; set; } = string.Empty;

	[Column(Unique = true)]
	public string Discriminator { get; set; } = string.Empty;

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	[Column(ExcludeInserts = true, ExcludeUpdates = true, OverrideValue = "CURRENT_TIMESTAMP")]
	public DateTime? CreatedAt { get; set; }

	[Column(ExcludeInserts = true, OverrideValue = "CURRENT_TIMESTAMP")]
	public DateTime? UpdatedAt { get; set; }

	public TestUserSqlite() { }

	public TestUserSqlite(string username, string discriminator, string firstname, string lastname)
	{
		UserName = username;
		Discriminator = discriminator;
		FirstName = firstname;
		LastName = lastname;
	}

	public const string CREATE_TABLE = @"
CREATE TABLE IF NOT EXISTS users (
	id INTEGER PRIMARY KEY,
	user_name TEXT NOT NULL,
	discriminator TEXT NOT NULL,	
	first_name TEXT NOT NULL,
	last_name TEXT NOT NULL,
	
	created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,

	UNIQUE (user_name, discriminator)
)";
}

public class SqliteExample(ISqlService sql, IQueryService query)
{
	private readonly ISqlService _sql = sql;
	private readonly IQueryService _query = query;

    public async Task Run()
	{
		//Cache your queries!
		var insertQuery = _query.Insert<TestUserSqlite>();
		var upsertQuery = _query.Upsert<TestUserSqlite>();
		var allQuery = _query.Select<TestUserSqlite>();
		var selectQuery = _query.Select<TestUserSqlite>(c => c.Prop(t => t.LastName));

		//Setup some data to use
		var john = new TestUserSqlite("test", "0001", "Jxhn", "Doe");
		var users = new[]
		{
			john,
			new TestUserSqlite("test", "0002", "Jane", "Doe"),
			new TestUserSqlite("test", "0003", "Jane", "Smith"),
			new TestUserSqlite("some", "0001", "Joe", "Smith")
		};

		//Add all users to the database
		foreach (var user in users)
			await _sql.Execute(insertQuery, user);

		//Upsert "Jxhn"s name
		john.FirstName = "John";
		await _sql.Execute(upsertQuery, john);

		//Get all people with the last name of "Smith"
		var allSmiths = await _sql.Get<TestUserSqlite>(selectQuery, new { LastName = "Smith" });

		Console.WriteLine("Here are all of the Smiths:");
		foreach (var user in allSmiths)
			Console.WriteLine("User: {0} - {1} {2} ({3}#{4})", user.Id, user.FirstName, user.LastName, user.UserName, user.Discriminator);

		//Get all of the records:
		var allUsers = await _sql.Get<TestUserSqlite>(allQuery);

		Console.WriteLine("Here are all users:");
		foreach (var user in allUsers)
			Console.WriteLine("User: {0} - {1} {2} ({3}#{4})", user.Id, user.FirstName, user.LastName, user.UserName, user.Discriminator);
	}

	public static SqliteExample Setup()
	{
		if (File.Exists("database.db"))
			File.Delete("database.db");

		return new ServiceCollection()
			.AddSqlService(c =>
			{
				c.AddSQLite("Data Source=database.db;", init: f =>
				{
					f.OnInit((con) => con.ExecuteAsync(TestUserSqlite.CREATE_TABLE));
				})
				.ConfigureGeneration(c => c.WithCamelCaseChange())
				.ConfigureTypes(c => c.CamelCase().Entity<TestUserSqlite>());
			})
			.AddSingleton<SqliteExample>()
			.BuildServiceProvider()
			.GetRequiredService<SqliteExample>();
	}
}
