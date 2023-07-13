namespace CardboardBox.Database.Tests;

using Generation;
using Models;

[TestClass]
public class DatabaseGenerationTests
{
	private IServiceProvider? _provider;

	[TestInitialize]
	public void Init()
	{
		_provider = new ServiceCollection()
			.AddSqlService((c) => { })
			.BuildServiceProvider();
	}

	[TestMethod]
	public void Query_Escape()
	{
		var srv = _provider?.GetRequiredService<IQueryGenerationService>();
		var cnf = _provider?.GetRequiredService<IQueryConfigProvider>();
		Assert.IsNotNull(srv);
		Assert.IsNotNull(cnf);

		var res = srv.Escape("Test", cnf.GetQueryConfig());
		Assert.AreEqual("[Test]", res);

		var dollarSignEscape = new QueryConfig("@", "$", "$", true);
		var noEscape = new QueryConfig { Escape = false };

		var dseRes = srv.Escape("Hello World", dollarSignEscape);
		Assert.AreEqual("$Hello World$", dseRes);

		var noRes = srv.Escape("How are you", noEscape);
		Assert.AreEqual("How are you", noRes);
	}

	[TestMethod]
	public void Query_Select()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var query = srv.Select<PropertyTestObject>(
			t => t.With(a => a.FirstName)
				  .With(a => a.LastName)
				  .Null(a => a.DeletedAt)
				  .NotNull(a => a.EmailAddress));

		Assert.AreEqual("SELECT * FROM [PropertyTestObject] " +
			"WHERE [FirstName] = @FirstName AND [LastName] = @LastName AND " +
			"[DeletedAt] IS NULL AND [Email] IS NOT NULL", query);
	}

	[TestMethod]
	public void Query_Fetch()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var query = srv.Fetch<PropertyTestObject>();

		Assert.AreEqual("SELECT * FROM [PropertyTestObject] WHERE [Id] = @Id", query);
	}

	[TestMethod]
	public void Query_Insert()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var query = srv.Insert<PropertyTestObject>();

		Assert.AreEqual("INSERT INTO [PropertyTestObject] " +
			"([UserName], [FirstName], [LastName], [Email], [DeletedAt]) VALUES " +
			"(@UserName, @FirstName, @LastName, @EmailAddress, @DeletedAt)", query);
	}

	[TestMethod]
	public void Query_Update()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var updatePk = srv.Update<PropertyTestObject>();
		var updateCol = srv.Update<PropertyTestObject>(t => t.With(a => a.EmailAddress));

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [UserName] = @UserName, [FirstName] = @FirstName, " +
			"[LastName] = @LastName, [Email] = @EmailAddress, " +
			"[UpdatedAt] = CURRENT_TIMESTAMP, [DeletedAt] = @DeletedAt " +
			"WHERE [Id] = @Id", updatePk);

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [UserName] = @UserName, [FirstName] = @FirstName, " +
			"[LastName] = @LastName, [Email] = @EmailAddress, " +
			"[UpdatedAt] = CURRENT_TIMESTAMP, [DeletedAt] = @DeletedAt " +
			"WHERE [Email] = @EmailAddress", updateCol);
	}

	[TestMethod]
	public void Query_Update_Only()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var updatePk = srv.UpdateOnly<PropertyTestObject>(
			t => t.Prop(a => a.EmailAddress).Prop(a => a.DeletedAt));
		var updateCol = srv.UpdateOnly<PropertyTestObject>(
			t => t.Prop(a => a.EmailAddress).Prop(a => a.DeletedAt), 
			t => t.With(a => a.EmailAddress));

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [Email] = @EmailAddress, [DeletedAt] = @DeletedAt " +
			"WHERE [Id] = @Id", updatePk);

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [Email] = @EmailAddress, [DeletedAt] = @DeletedAt " +
			"WHERE [Email] = @EmailAddress", updateCol);
	}

	[TestMethod]
	public void Query_Delete()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var deletePk = srv.Delete<PropertyTestObject>();
		var deleteCol = srv.Delete<PropertyTestObject>(t => t.With(a => a.EmailAddress));

		Assert.AreEqual("DELETE FROM [PropertyTestObject] WHERE [Id] = @Id", deletePk);
		Assert.AreEqual("DELETE FROM [PropertyTestObject] WHERE [Email] = @EmailAddress", deleteCol);
	}

	[TestMethod]
	public void Query_Paginate()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var query = srv.Paginate<PropertyTestObject, DateTime>(t => t.UpdatedAt);
		var queryWhere = srv.Paginate<PropertyTestObject, string>(
			t => t.FirstName, false,
			t => t.With(a => a.EmailAddress));

		Assert.AreEqual("SELECT * FROM [PropertyTestObject] " +
			"ORDER BY [UpdatedAt] ASC " +
			"OFFSET @offset ROWS " +
			"FETCH NEXT @limit ROWS ONLY; " +
			"SELECT COUNT(*) FROM [PropertyTestObject];", query);

		Assert.AreEqual("SELECT * FROM [PropertyTestObject] " +
			"WHERE [Email] = @EmailAddress " +
			"ORDER BY [FirstName] DESC " +
			"OFFSET @offset ROWS " +
			"FETCH NEXT @limit ROWS ONLY; " +
			"SELECT COUNT(*) FROM [PropertyTestObject] " +
			"WHERE [Email] = @EmailAddress;", queryWhere);
	}

	[TestMethod]
	public void Query_Upsert()
	{
		var srv = _provider?.GetRequiredService<IQueryService>();
		Assert.IsNotNull(srv);

		var upsertUq = srv.Upsert<PropertyTestObject>();
		var upsertWhere = srv.Upsert<PropertyTestObject>(t => t.Prop(a => a.FirstName).Prop(a => a.EmailAddress));

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [UserName] = @UserName, [FirstName] = @FirstName, " +
			"[LastName] = @LastName, [Email] = @EmailAddress, " +
			"[UpdatedAt] = CURRENT_TIMESTAMP, [DeletedAt] = @DeletedAt " +
			"WHERE [UserName] = @UserName; " +
			"IF (@@ROWCOUNT = 0) " +
			"INSERT INTO [PropertyTestObject] " +
			"([UserName], [FirstName], [LastName], [Email], [DeletedAt]) VALUES " +
			"(@UserName, @FirstName, @LastName, @EmailAddress, @DeletedAt);", upsertUq);

		Assert.AreEqual("UPDATE [PropertyTestObject] " +
			"SET [UserName] = @UserName, [FirstName] = @FirstName, " +
			"[LastName] = @LastName, [Email] = @EmailAddress, " +
			"[UpdatedAt] = CURRENT_TIMESTAMP, [DeletedAt] = @DeletedAt " +
			"WHERE [FirstName] = @FirstName AND [Email] = @EmailAddress; " +
			"IF (@@ROWCOUNT = 0) " +
			"INSERT INTO [PropertyTestObject] " +
			"([UserName], [FirstName], [LastName], [Email], [DeletedAt]) VALUES " +
			"(@UserName, @FirstName, @LastName, @EmailAddress, @DeletedAt);", upsertWhere);
	}

	[TestMethod]
	public void Query_Escape_CustomTable()
	{
		var srv = _provider?.GetRequiredService<IQueryGenerationService>();
		var cnf = _provider?.GetRequiredService<IQueryConfigProvider>();
		var rfl = _provider?.GetRequiredService<IReflectedService>();
		Assert.IsNotNull(srv);
		Assert.IsNotNull(cnf);
		Assert.IsNotNull(rfl);

		var type = rfl.GetType<ModifiedTableObject>();
		var name = srv.Escape(type.Name, cnf.GetQueryConfig());

		Assert.AreEqual("[HelloWorld].[How].[AreYou]", name);
	}
}