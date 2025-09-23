using System.Net;
using System.Net.Http.Json;
using Auth.API.Controller;
using FluentAssertions;
using HelloTest.DBFixture;
using Xunit.Abstractions;

namespace HelloTest.Test.Auth.API;

// [CollectionDefinition("DB container")]
// public class PostgresCollection : ICollectionFixture<PostgresFixture>;

[Collection("DB container")]
public class AuthApiTest : IClassFixture<AuthWebAppFactory>
{
    private readonly AuthWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _testOutputHelper;

    private string _adminToken;

    public AuthApiTest(AuthWebAppFactory factory, ITestOutputHelper testOutputHelper)
    {
        // code run for each test
        // but the factory is created only once, shared across 3 tests
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        
        _testOutputHelper.WriteLine("Creating Auth API Test");
    }
    
 
    
    [Fact]
    public async Task RegisterUserTest()
    {

        // init this way because LoginDto is a Record
        var registerPayload = new RegisterDto("myUser@gmail.com", "myPassword");
        
        var responseRegister = await _client.PostAsJsonAsync("/auth/register", registerPayload);
        
        responseRegister.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var loginPayload = new LoginDto("myUser@gmail.com", "myPassword");
        
        var responseLogin = await _client.PostAsJsonAsync("/auth/login", loginPayload);
        
        responseLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var response = await _client.GetAsync("/auth");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetAllUsersTest() 
    {
        
        var response = await _client.GetAsync("/auth");
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task LoginAdmin_valid()
    {

        // init this way because LoginDto is a Record
        var payload = new LoginDto("myUser@gmail.com", "myPassword");
        
        
        var response = await _client.PostAsJsonAsync("/auth/login", payload);
       
        
        _adminToken = response.Content.ReadAsStringAsync().Result;
        
        _testOutputHelper.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task LoginAdmin_wrongPassword()
    {

        // init this way because LoginDto is a Record
        var payload = new LoginDto("admin@gmail.com", "wrongpassword");
        
        
        var response = await _client.PostAsJsonAsync("/auth/login", payload);
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
}