using HelloTest.Test.ProductAPI.ServicesUnitest.GrpcService;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Order.API.Dtos;
using Order.API.Interfaces;
using Order.API.RabbitMqService;
using Product;
using ProductAPI.GRPC.Service;
using ProductAPI.Interfaces;
using SharedLibrary.MassTransit.RabbitMQ;
using Xunit.Abstractions;

namespace HelloTest.Test.Order.API.MassTransitConsumer;

public class TestMtPaymentResultConsumer : IClassFixture<CustomOrderAppFactory>, IAsyncLifetime
{
    private CustomOrderAppFactory _factory;
    private Mock<ProductService.ProductServiceClient> _mockGrpcProductClient;
    private ITestHarness _harness;
    private IBus _bus;
    private IServiceScope _scope;

    private ITestOutputHelper _testOutputHelper;
    private Mock<IOrderRepository> _mockOrderRepo;
    
    // captured parameter
    private PutOrderDto _capturedPutOrderDto;

    public TestMtPaymentResultConsumer(ITestOutputHelper testOutputHelper, CustomOrderAppFactory factory)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
    }


    public async Task InitializeAsync()
    {
        var httpClient = _factory.CreateDefaultClient();
        
        _mockOrderRepo = _factory._mockProductRepo;
        
        // setup 
        
        // capture parameter passed to UpdateAsync
     
        
        _mockGrpcProductClient = _factory._mockGrpcClient;
        
        _scope = _factory.Services.CreateScope();
        _harness = _scope.ServiceProvider.GetRequiredService<ITestHarness>();
        _bus =  _scope.ServiceProvider.GetRequiredService<IBus>();
        
        await _harness.Start();
        
        // setup client
        
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        _scope.Dispose();
    }

    [Fact]
    public async Task SendPaymentSuccess()
    {
        // send success payment result
        var message = new PaymentResultMsg
        {
            OrderId = Guid.NewGuid(),
            Success = true
        };
        
        _mockOrderRepo
            .Setup(repo => repo.updateOrderStatus(It.IsAny<Guid>(), It.IsAny<PutOrderDto>()))
            .Callback<Guid, PutOrderDto>((guid, dto) =>
            {
                _capturedPutOrderDto = dto;
            });
        
        await _bus.Publish(message);
        
        var consumerHarness = _factory.Services.GetRequiredService<IConsumerTestHarness<MtPaymentResultConsumer>>();
        Assert.True(await consumerHarness.Consumed.Any<PaymentResultMsg>());
        
        _mockOrderRepo.Verify( r => r.getByIdAsync(It.IsAny<Guid>()), Times.Exactly(1));
        
        _mockOrderRepo.Verify( r => r.updateOrderStatus(
                It.IsAny<Guid>(),
                It.Is<PutOrderDto>( p => p.OrderStatus == "Confirmed"))
            , Times.Exactly(1));
        
        Assert.Equal("Confirmed", _capturedPutOrderDto.OrderStatus);
        
    }
    
    [Fact]
    public async Task SendPaymentFailure()
    {
        // send success payment result
        var message = new PaymentResultMsg
        {
            OrderId = Guid.NewGuid(),
            Success = false
        };
        
        await _bus.Publish(message);
        
        var consumerHarness = _factory.Services.GetRequiredService<IConsumerTestHarness<MtPaymentResultConsumer>>();
        Assert.True(await consumerHarness.Consumed.Any<PaymentResultMsg>());
        
        _mockOrderRepo.Verify( r => r.getByIdAsync(It.IsAny<Guid>()), Times.Exactly(1));
        
        _mockOrderRepo.Verify( r => r.updateOrderStatus(
            It.IsAny<Guid>(),
            It.Is<PutOrderDto>( p => p.OrderStatus == "Payment Failed"))
            , Times.Exactly(1));
        
    }

}