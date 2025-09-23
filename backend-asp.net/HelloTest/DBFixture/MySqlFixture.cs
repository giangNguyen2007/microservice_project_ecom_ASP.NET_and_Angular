using Testcontainers.MySql;

namespace HelloTest.DBFixture;

public class MySqlFixture : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;
    
    public string _connectionString {get; set;}

    public MySqlFixture()
    {
        _mySqlContainer = new MySqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();
        
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
        
        _connectionString =  _mySqlContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _mySqlContainer.DisposeAsync();
    }
}