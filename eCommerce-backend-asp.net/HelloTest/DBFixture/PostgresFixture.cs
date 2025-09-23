using Testcontainers.PostgreSql;

namespace HelloTest.DBFixture;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("helloDotNet")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string _connectionString {get; set;}
    
    public async Task InitializeAsync()
    {
        
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();
        
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync().AsTask();
    }
}