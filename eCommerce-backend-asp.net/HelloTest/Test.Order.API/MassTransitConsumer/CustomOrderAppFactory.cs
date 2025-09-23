using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Order.API;
using Order.API.Interfaces;
using Order.API.RabbitMqService;
using Product;
using ProductAPI.Interfaces;

namespace HelloTest.Test.Order.API.MassTransitConsumer;

public class CustomOrderAppFactory : WebApplicationFactory<OrderApiProgram>
{
    public Mock<IOrderRepository> _mockProductRepo;
    public Mock<ProductService.ProductServiceClient> _mockGrpcClient;
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // remove existing IProductRepository
            var descriptor = services.Single(d => d.ServiceType == typeof(IOrderRepository));
            services.Remove(descriptor);
            
            // _mockRepo.Object is an instance of a class implementing IProductRepo
            _mockProductRepo = new Mock<IOrderRepository>();
            
            // input = lamda function, returning a object implementing IProductRepo
            services.AddScoped<IOrderRepository>(_ => _mockProductRepo.Object);
            
            // ============ Replace gRPC client by mocked one
            // remove grpc Service Client
            var descriptor2 = services.Single(d => d.ServiceType == typeof(ProductService.ProductServiceClient));
            services.Remove(descriptor2);
            
            _mockGrpcClient = new Mock<ProductService.ProductServiceClient>();
            services.AddScoped<ProductService.ProductServiceClient>(_ => _mockGrpcClient.Object);
            
            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MtPaymentResultConsumer>();
            });
            
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        return base.CreateHost(builder);
    }
}