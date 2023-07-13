using CardboardBox;
using CardboardBox.Database;
using CardboardBox.Database.Generation;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ExampleApp;

[Table("postgres_test_users")]
public class TestUserPostgres
{
	[Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
	public long Id { get; set; }

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

	public TestUserPostgres() { }

	public TestUserPostgres(string username, string discriminator, string firstname, string lastname)
	{
		UserName = username;
		Discriminator = discriminator;
		FirstName = firstname;
		LastName = lastname;
	}

	public const string CREATE_TABLE = @"
CREATE TABLE IF NOT EXISTS postgres_test_users (
	id BIGSERIAL PRIMARY KEY,

	user_name TEXT NOT NULL,
	discriminator TEXT NOT NULL,	
	first_name TEXT NOT NULL,
	last_name TEXT NOT NULL,
	
	created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,

	CONSTRAINT uiq_users_username_discriminator UNIQUE (user_name, discriminator)
)";
}

public class PostgresExample
{
	private readonly ISqlService _sql;
	private readonly IQueryService _query;

	public PostgresExample(ISqlService sql, IQueryService query)
	{
		_sql = sql;
		_query = query;
	}

	public async Task Run()
	{
		//Cache your queries!
		var insertQuery = _query.Insert<TestUserPostgres>();
		var upsertQuery = _query.Upsert<TestUserPostgres>();
		var allQuery = _query.Select<TestUserPostgres>();
		var selectQuery = _query.Select<TestUserPostgres>(c => c.Prop(t => t.LastName));

		//Setup some data to use
		var john = new TestUserPostgres("test", "0001", "Jxhn", "Doe");
		var users = new[]
		{
			john,
			new TestUserPostgres("test", "0002", "Jane", "Doe"),
			new TestUserPostgres("test", "0003", "Jane", "Smith"),
			new TestUserPostgres("some", "0001", "Joe", "Smith")
		};

		//Add all users to the database
		foreach (var user in users)
			await _sql.Execute(insertQuery, user);

		//Upsert "Jxhn"s name
		john.FirstName = "John";
		await _sql.Execute(upsertQuery, john);

		//Get all people with the last name of "Smith"
		var allSmiths = await _sql.Get<TestUserPostgres>(selectQuery, new { LastName = "Smith" });

		Console.WriteLine("Here are all of the Smiths:");
		foreach (var user in allSmiths)
			Console.WriteLine("User: {0} - {1} {2} ({3}#{4})", user.Id, user.FirstName, user.LastName, user.UserName, user.Discriminator);

		//Get all of the records:
		var allUsers = await _sql.Get<TestUserPostgres>(allQuery);

		Console.WriteLine("Here are all users:");
		foreach (var user in allUsers)
			Console.WriteLine("User: {0} - {1} {2} ({3}#{4})", user.Id, user.FirstName, user.LastName, user.UserName, user.Discriminator);
	}

	public static PostgresExample Setup()
	{
		return new ServiceCollection()
			.AddConfig(c =>
			{
				c.AddFile("appsettings.json", false, true)
				 .AddEnvironmentVariables();
			})
			.AddSqlService(c =>
			{
				c.AddPostgres<ConfigurationSqlConfig>(init: f =>
				{
					f.OnInit(con => con.ExecuteAsync(TestUserPostgres.CREATE_TABLE));
				})
				.ConfigureGeneration(c => c.WithCamelCaseChange())
				.ConfigureTypes(c => c.CamelCase().Entity<TestUserPostgres>());
			})
			.AddSingleton<PostgresExample>()
			.BuildServiceProvider()
			.GetRequiredService<PostgresExample>();
	}

	public class ConfigurationSqlConfig : ISqlConfig<NpgsqlConnection>
	{
		private readonly IConfiguration _config;

		public int Timeout { get; set; } = 0;

		public string ConnectionString => _config["Database:Postgres"];

		public ConfigurationSqlConfig(IConfiguration config)
		{
			_config = config;
		}
	}
}
