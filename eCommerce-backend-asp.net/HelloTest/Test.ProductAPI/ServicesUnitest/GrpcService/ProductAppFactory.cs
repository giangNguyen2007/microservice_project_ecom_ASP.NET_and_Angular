using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductAPI;
using ProductAPI.Data;
using ProductAPI.Interfaces;
using ProductAPI.Repository;

namespace HelloTest.Test.ProductAPI.ServicesUnitest.GrpcService;


// Instanctiated once for test class
public class ProductAppFactory : WebApplicationFactory<ProductApiProgram>, IAsyncLifetime
{
    public Mock<IProductRepository> _mockProductRepo;
    
    
    // run only when the factory create the Web host
    // before this, _mockProductRepo = null
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.Single(d => d.ServiceType == typeof(IProductRepository));
            services.Remove(descriptor);

            
            // _mockRepo.Object is an instance of a class implementing IProductRepo
            _mockProductRepo = new Mock<IProductRepository>();
            
            // input = lamda function, returning a object implementing IProductRepo
            services.AddScoped<IProductRepository>(_ => _mockProductRepo.Object);

        }); // optional
    }

    // called immediately after class instance creation
    public async Task InitializeAsync()
    {
        return ;
    }

    public async Task DisposeAsync()
    {
        return;
    }
}