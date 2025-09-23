using HelloTest.DBFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductAPI;
using ProductAPI.Data;
using ProductAPI.Model;

namespace HelloTest.Test.ProductAPI;

public class CustomProductAPIFactory : WebApplicationFactory<ProductApiProgram>
{
    private string _connectionString;
    private IDbContextTransaction _transaction;
    private ProductDBContext _dbContext;
    private IServiceScope _testScope;

    public CustomProductAPIFactory( )
    {
        // _connectionString = fixture._connectionString;
    }

    // Run before webhost creation, at Factory instance creation
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove default DbContext
            // var descriptor = services.SingleOrDefault(
            //     d => d.ServiceType == typeof(DbContextOptions<ProductDBContext>));
            // if (descriptor != null)
            //     services.Remove(descriptor);
            
            //var myConnection = "Server=localhost;Database=mydatabase;User=myuser;Password=mypassword;";
            
            // Register with your existing DB
            
            var provider = services.BuildServiceProvider();

            using var tempScope = provider.CreateScope();
            var dbOptions = tempScope.ServiceProvider.GetRequiredService<DbContextOptions<ProductDBContext>>();

            // Build the shared context
            _dbContext = new ProductDBContext(dbOptions);
            _dbContext.Database.OpenConnection();
            _transaction = _dbContext.Database.BeginTransaction();
            
            services.AddScoped<ProductDBContext>(_ => _dbContext);
            
        
            
        }); // optional
    }
    
    public void BeginTestScope()
    {
        // 👇 Create scope AFTER factory is fully initialized
        // _testScope = Services.CreateScope();
        //     
        // _dbContext = _testScope.ServiceProvider.GetRequiredService<ProductDBContext>();
        //
        //
        //     
        // _dbContext.Database.OpenConnection();
            
        // only needed if running against Isolated, Fresh DB
        //_dbContext.Database.Migrate();  
            
        // _transaction = _dbContext.Database.BeginTransaction();
        //     
        // Console.WriteLine("Transaction Started");
            
        // _dbContext.Products.AddRange(
        //     new ProductModel
        //     {
        //         Id = Guid.NewGuid(),
        //         Title = "Nike Shirt",
        //         Description = "Nike Shirt",
        //         Price = 50,
        //         Stock = 100,
        //         Category = "Shirt"
        //     }
        //          
        // );
        //_dbContext.SaveChanges();
        
    }
    
    public async Task RollbackTransactionAsync()
    {
        try
        {
            _transaction?.Rollback();
            _dbContext?.Database.CloseConnection();
        }
        finally
        {
            _transaction?.Dispose();
            _dbContext = null;
        }
    }
}