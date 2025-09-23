using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order.API.Dtos;
using Xunit.Abstractions;

namespace HelloTest.Test.Order.API;

public class OrderAPITest : IClassFixture<CustomOrderWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _testOutputHelper;
    
    public OrderAPITest( CustomOrderWebAppFactory factory, ITestOutputHelper testOutputHelper)
    {
        _client = factory.CreateClient();
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact()]
    public async Task GetAllOrders() 
    {
        
        var response = await _client.GetAsync("/orders");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact()]
    public async Task PlaceOneOrder_validProductId()
    {

        var payload = new PostOrderDto()
        {
            userEmail = Guid.NewGuid(),
            ProductId = Guid.Parse("0193bf62-dc7f-45c9-8667-3ee7ce5a7e11"),
            Quantity = 1,
        };
        
        var postResponse = await _client.PostAsJsonAsync("/orders", payload);
        
        _testOutputHelper.WriteLine(postResponse.Content.ReadAsStringAsync().Result);
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact()]
    public async Task PlaceOneOrder_inValidProductId()
    {

        var payload = new PostOrderDto()
        {
            userEmail = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
        };
        
        var postResponse = await _client.PostAsJsonAsync("/orders", payload);
        
        _testOutputHelper.WriteLine(postResponse.Content.ReadAsStringAsync().Result);
        postResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact()]
    public async Task PlaceOneOrder_notEnoughStock()
    {

        var payload = new PostOrderDto()
        {
            userEmail = Guid.NewGuid(),
            ProductId = Guid.Parse("0193bf62-dc7f-45c9-8667-3ee7ce5a7e11"),
            Quantity = 10000,
        };
        
        var postResponse = await _client.PostAsJsonAsync("/orders", payload);
        
        _testOutputHelper.WriteLine(postResponse.Content.ReadAsStringAsync().Result);
        
        // extract response message
        var responseBody = await postResponse.Content.ReadAsStringAsync();
        var error = JObject.Parse(responseBody);
        
        
        // Assertion
        postResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        error["error"]?.ToString().Should().Be("Not enough stock");
    }
    
    
}