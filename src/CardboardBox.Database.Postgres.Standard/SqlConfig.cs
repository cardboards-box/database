namespace CardboardBox.Database.Postgres.Standard;

internal class SqlConfig(IConfiguration config) : ISqlConfig<NpgsqlConnection>
{
    private readonly IConfiguration _config = config;

    public string ConnectionString =>
        _config["Database:ConnectionString"]
            ?? throw new NullReferenceException("Database:ConnectionString - Required setting is not present");

    public int Timeout => int.TryParse(_config["Database:Timeout"], out int timeout) ? timeout : 0;
}

