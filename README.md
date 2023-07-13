# CardboardBox.Database
A wrapper around Dapper and various ADO.Net providers to connect to different SQL Engines with built in basic query generation.

## Installation
You will probably want to install one of the ADO.net provider specific packages:

Install Postgres via Nuget:
```
PM> Install-Package CardboardBox.Database.Postgres
```

Install SQLite via Nuget:
```
PM> Install-Package CardboardBox.Database.SQLite
```

Install MSSQL / SQL Server via Nuget: (Coming soon)
```
PM> Install-Package CardboardBox.Database.MSSQL
```

## Setup
This package is designed to work with `Microsoft.Extensions.DependencyInjection`:

```csharp
using CardboardBox.Database;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSqlService(c => 
{
	//Configure the SQL engines you want to use
	c.AddPostgres("<connection string>");

	c.AddSqlite("<connection string>");
});
```

## Querying
To use the database engine, you can inject the `ISqlService` interface into any of your services:
```csharp
public class SomeService 
{
	private readonly ISqlService _sql;

	public SomeService(ISqlService sql) { _sql = sql; }

	public Task<User> Fetch(long id) => _sql.Fetch<User>("SELECT * FROM users WHERE id = @id", new { id });

	public Task<User[]> Get() => _sql.Get<User>("SELECT * FROM users");

	public async Task DoSomethingComplex()
	{
		using var con = await _sql.CreateConnection();
		//Do something with the active connection
	}
}
```

## Query Generation
Query Generation is centered around the `IQueryService` that generates queries based on your POCOs.
Said generation uses property names, Dapper Fluent, and optional attributes to generate SQL queries.

### Basic Usage - Attributes
There are 2 primary attributes within the generation scheme: `TableAttribute` and `ColumnAttribute`. 
You can use them like so:
```csharp
[Table("Users")]
public class User 
{
	[Column(PrimaryKey = true, ExcludeInserts = true, ExcludeUpdates = true)]
	public int Id { get; set; }

	[Column(Unique = true)]
	public string UserName { get; set; } = string.Empty;

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;
}
```

This will create the following queries when used against the `IQueryService`:
```sql
--IQueryService.Select<User>(t => t.With(a => a.FirstName));
SELECT * FROM [Users] WHERE [FirstName] = @FirstName;

--IQueryService.Fetch<User>();
SELECT * FROM [Users] WHERE [Id] = @Id;

--IQueryService.Insert<User>();
INSERT INTO [Users] ([UserName], [FirstName], [LastName]) VALUES (@UserName, @FirstName, @LastName);
```

### Basic Usage - Changing Column Case Convention

You can also change the case convention during configuration and it will automatically change the case of the column names:
```csharp
services.AddSqlService(c => 
{
	//Configure the SQL engines you want to use
	c.AddPostgres("<connection string>")
	 //This tells the query generation system to use camel_case names
	 .ConfigureGeneration(c => c.WithCamelCaseChange())
	 //This tells Dapper Fluent to use camel_case for resolving property names for the `User` class
	 .ConfigureTypes(c => 
		c.CamelCase()
		 .Entity<User>());
});
```

This will create the following queries when used against the `IQueryService`:
```sql
--IQueryService.Select<User>(t => t.With(a => a.FirstName));
SELECT * FROM "users" WHERE "first_name" = :FirstName;

--IQueryService.Fetch<User>();
SELECT * FROM "users" WHERE "id" = :Id;

--IQueryService.Insert<User>();
INSERT INTO "users" ("user_name", "first_name", "last_name") VALUES (:UserName, :FirstName, :LastName);
```

