using Microsoft.EntityFrameworkCore;
using Payment.API.data;
using Xunit;
using Xunit.Abstractions;

namespace Payment.API.RabbitMqServices;

public class DbFixture
{
    
    public PaymentRequestDbContext DbContext;
    
    public string _dbPath;
    
    public  PaymentRequestModel myEntity = new PaymentRequestModel
    {
        OrderId = Guid.NewGuid(),
        Amount = 100,
        BankAccountId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
    };

    public DbFixture()
    {
    
        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var _dbPath = System.IO.Path.Combine(rootPath, "paymentApi_sqlite.db");
        
        // verbatim string => The @ before a string in C# makes it a verbatim string literal, so backslashes (\) are treated as normal characters, not escape sequences
        string absolutePath = @"C:\Users\raira\RiderProjects\HelloDotNet\Payment.API\paymentApi_sqlite_test.db";
        var options = new DbContextOptionsBuilder<PaymentRequestDbContext>()
            .UseSqlite($"Data Source={absolutePath}")
            .Options;
        
    
        DbContext = new PaymentRequestDbContext(options);
        
        // create the .db file with the tables defined in DbContext, if the db does not exist
        DbContext.Database.EnsureCreated();
        
        
    }
}

public class DbContextTest : IClassFixture<DbFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    
    private PaymentRequestDbContext _dbContext;
    
    private PaymentRequestModel _myEntity;
    
    private DbFixture _dbFixture;
    
    
    
    public DbContextTest(DbFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _dbContext = fixture.DbContext;
        _testOutputHelper = testOutputHelper;
        _myEntity = fixture.myEntity;
        _dbFixture = fixture;
    }

    [Fact]
    public async Task TestDbContext()
    {
        
        _dbContext.PaymentRequests.Add(_myEntity);
        await _dbContext.SaveChangesAsync();
        
        var retrievedEntity = await _dbContext.PaymentRequests.FindAsync(_myEntity.OrderId);
        Assert.NotNull(retrievedEntity);
        _testOutputHelper.WriteLine(retrievedEntity.BankAccountId.ToString());
    }
    
    [Fact]
    public async Task TestDbContext2()
    {
        
        var retrievedEntity = await _dbContext.PaymentRequests.FindAsync(_myEntity.OrderId);
        Assert.NotNull(retrievedEntity);
        _testOutputHelper.WriteLine(retrievedEntity.BankAccountId.ToString());
    }
    
    [Fact]
    public async Task TestDbContext3()
    {
        //_testOutputHelper.WriteLine(_dbFixture._dbPath);
        // retrieve all entities
        var allEntities = await _dbContext.PaymentRequests.ToListAsync();
        //Assert.NotEmpty(allEntities);
        _testOutputHelper.WriteLine($"Total entities: {allEntities.Count}");
    }
    
    
}