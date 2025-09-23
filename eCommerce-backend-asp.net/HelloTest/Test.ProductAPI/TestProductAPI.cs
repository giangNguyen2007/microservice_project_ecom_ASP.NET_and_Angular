using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HelloTest.DBFixture;
using HelloTest.Test.Auth.API;
using ProductAPI.Dtos.Game;
using ProductAPI.Model;
using Xunit.Abstractions;

namespace HelloTest.Test.ProductAPI;

// [CollectionDefinition("DB container")]
// public class PostgresCollection : ICollectionFixture<MySqlFixture>;

[Collection("DB container")]
public class TestProductAPI : IClassFixture<CustomProductAPIFactory>, IAsyncLifetime
{
    private HttpClient _client;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly CustomProductAPIFactory _factory;
 
  
    public TestProductAPI( CustomProductAPIFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
       
        _testOutputHelper = testOutputHelper;
    }
    
    public Task InitializeAsync()
    {
        _testOutputHelper.WriteLine("Initializing Database");
        _factory.BeginTestScope();
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _testOutputHelper.WriteLine("Rollback database");
        await _factory.RollbackTransactionAsync();
    }

    [Fact]
    public async Task GetAllProducts() 
    {
        
        var response = await _client.GetAsync("/product");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetSingleProduct_OK() 
    {
        
        var response = await _client.GetAsync("/product/0193bf62-dc7f-45c9-8667-3ee7ce5a7e11");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetSingleProduct_WrongId() 
    {
        
        var response = await _client.GetAsync("/product/0193bf62-dc7f-45c9-8667-3");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task PostSingleProduct()
    {
        var payload = new PostProductDto()
        {
            Title = "Adidas Ball 2",
            Description = "Adidas Ball 2",
            Price = 100,
            Stock = 500,
            Category = "Balls 2"
        };
        var postResponse = await _client.PostAsJsonAsync("/product", payload);
        //postResponse.EnsureSuccessStatusCode();

        //var newGame = await postResponse.Content.ReadFromJsonAsync<ProductModel>();
        //_testOutputHelper.WriteLine(postResponse.Content.ReadAsStringAsync().Result);
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
    }
    
    [Fact]
    public async Task PostSingleProduct_WrongModel()
    {
        var payload = new ProductModel()
        {
            Title = "Adidas Ball",
            Description = "Adidas Ball",
            Price = 100,
            Stock = 500,
            Category = "Balls"
        };
        var postResponse = await _client.PostAsJsonAsync("/product", payload);
        postResponse.EnsureSuccessStatusCode();

        var newGame = await postResponse.Content.ReadFromJsonAsync<ProductModel>();
        _testOutputHelper.WriteLine(postResponse.Content.ReadAsStringAsync().Result);
        postResponse.StatusCode.Should().NotBe(HttpStatusCode.OK);
        
    }


   
}