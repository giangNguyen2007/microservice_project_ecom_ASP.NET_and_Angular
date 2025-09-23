using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Product;
using ProductAPI.Controllers;
using ProductAPI.GRPC.Service;
using ProductAPI.Interfaces;
using ProductAPI.Model;
using Xunit.Abstractions;
using ProductRequest = YourApp.Grpc.ProductRequest;

namespace HelloTest.Test.ProductAPI.ServicesUnitest.GrpcService;

    
public class GrpcProductServerUnitTest: IClassFixture<ProductAppFactory>, IAsyncLifetime
{
    private ProductAppFactory _factory;
    private ProductService.ProductServiceClient _grpcClient;
    private IServiceScope _scope;
    private GrpcProductServer _grpcServer;
    private ITestOutputHelper _testOutputHelper;
    private Mock<IProductRepository> _mockProductRepo;
    
    private Guid _myProductId = Guid.NewGuid();
    
    

    public GrpcProductServerUnitTest( ProductAppFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
      
        _testOutputHelper = testOutputHelper;
        
    }
    
    // run after Test class Constructor
    public async Task InitializeAsync()
    {
        // create client, thus create the Web host => run ConfigureWebHost
        var httpClient = _factory.CreateDefaultClient();
        
        
        // how _mockRepo is no longer null
        _mockProductRepo = _factory._mockProductRepo;
        
        // create channel pointing to httpClient
        var myChannel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient });
        
        // create grpc client service pointing to the channel
        _grpcClient = new ProductService.ProductServiceClient(myChannel);

        // extract the grpc server
        _scope = _factory.Services.CreateScope();
        _grpcServer = _scope.ServiceProvider.GetService<GrpcProductServer>();

        if (_grpcServer == null)
        {
            _testOutputHelper.WriteLine("_grpcServer == null");
        }
        
        // setup the mock repo
        
        
        // var fakePoduct = new Bogus.Faker<ProductModel>().Generate();

        var fakePoduct = new ProductModel()
        {
            Id = _myProductId,
            Title = "Ergonomic Office Chair",
            Description = "Adjustable mesh office chair with lumbar support",
            PhotoUrl = "https://example.com/images/office_chair.jpg",
            Category = "Furniture",
            Price = 210,
            Stock = 5
        };
        
        _testOutputHelper.WriteLine($"fakePoduct == {fakePoduct.Title}");
        
        // if called with myId, return a good response
        _mockProductRepo.Setup(r => r.getByIdAsync(It.Is<Guid>(id => id == _myProductId)))
            .ReturnsAsync(fakePoduct);

    }
    
    
    // run after test finishes
    public async Task DisposeAsync()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task testGrpcServerDirect()
    {

        // create mocked server call context
        var mockContext = new Mock<ServerCallContext>();
        // setup the context if necessary
        
        var prodRequest = new ProductRequest(){ ProductId = "wrong id"};
        
        // wrong guid
        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
        {
            var reply = await _grpcServer.GetProductInfo(prodRequest, mockContext.Object);
        });
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("Invalid GUID format", exception.Status.Detail);
        
        // ======= TEST 2 ========
        // good Id, but product not found
        var prodRequest2 = new ProductRequest(){ ProductId = Guid.NewGuid().ToString()};
        var reply2 = await _grpcServer.GetProductInfo(prodRequest2, mockContext.Object);
        
        
        // assert the function is called once
        _mockProductRepo.Verify(r => r.getByIdAsync(It.IsAny<Guid>()), Times.Once);
        Assert.False(reply2.Found);  
        
        
        // ====== TEST 3 =======
        
        
        var prodRequest3 = new ProductRequest(){ ProductId = _myProductId.ToString()};
        var reply3 = await _grpcServer.GetProductInfo(prodRequest3, mockContext.Object);
        
        // assert the function is called once with myId
        _mockProductRepo.Verify(r => r.getByIdAsync(It.Is<Guid>( id => id == _myProductId)), Times.Once);
        Assert.True(reply3.Found);  
        
    }

    [Fact]
    public async Task testGrpcServerByWebServer()
    {
        // cannot mock CallOption, because it is a struct, not an interface
        // var mockCallOption = new Mock<CallOptions>();
        
        var myCallOptions = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(10));
        
        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
        {
            var reply = await _grpcClient.GetProductInfoAsync(
                new Product.ProductRequest() { ProductId = "wrong id" },
                myCallOptions
            );
        });
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("Invalid GUID format", exception.Status.Detail);
        
        
        // ======= TEST 2 ========
        // good Id, but product not found
        var prodRequest2 = new Product.ProductRequest(){ ProductId = Guid.NewGuid().ToString()};
        var reply2 = await _grpcClient.GetProductInfoAsync(prodRequest2, myCallOptions);
        
        // assert the function is called once
        _mockProductRepo.Verify(r => r.getByIdAsync(It.IsAny<Guid>()), Times.Once);
        Assert.False(reply2.Found);  
        
        // ==== Test 3 ====
        // valid Id, product found
        
        var prodRequest3 = new Product.ProductRequest(){ ProductId = _myProductId.ToString()};
        var reply3 = await _grpcClient.GetProductInfoAsync(prodRequest3, myCallOptions);
        
        // assert the function is called once with myId
        _mockProductRepo.Verify(r => r.getByIdAsync(It.Is<Guid>( id => id == _myProductId)), Times.Once);
        Assert.True(reply3.Found);  
        
    }






}