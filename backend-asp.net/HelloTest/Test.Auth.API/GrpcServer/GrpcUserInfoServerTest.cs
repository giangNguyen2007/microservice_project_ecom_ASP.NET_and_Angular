using System.Net;
using Auth.API;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using MyGrpc;
using Xunit.Abstractions;

namespace HelloTest.Test.Auth.API.GrpcServer;

public class GrpcUserInfoServerTest : IClassFixture<WebApplicationFactory<AuthApiProgram>> , IAsyncLifetime
{
    
    private WebApplicationFactory<AuthApiProgram> _factory;
    private ITestOutputHelper _testOutputHelper;
    private UserService.UserServiceClient _grpcClient;
    private HttpClient _httpClient;

    public GrpcUserInfoServerTest(ITestOutputHelper testOutputHelper, WebApplicationFactory<AuthApiProgram> factory)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        //_factory = new WebApplicationFactory<AuthApiProgram>();
        
        _httpClient = _factory.CreateClient();
        
        
        
        // create GRPC channel pointing to the webapp
        var myChannel = GrpcChannel.ForAddress(_httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = _httpClient });
        
        // create grpc client service pointing to the channel
        _grpcClient = new UserService.UserServiceClient(myChannel);
    }

    public async Task DisposeAsync()
    {
        _factory?.Dispose();
    }

    [Fact]
    public async Task RunAsync()
    {
        var myCallOptions = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(10));
        var reply = await _grpcClient.GetUserInfoAsync(new UserRequest(){UserEmail = "myEmail"}, myCallOptions);
        
        Assert.NotNull(reply);
        _testOutputHelper.WriteLine(reply.Found.ToString());
    }
    
    [Fact]
    public async Task TestHttp()
    {
        var response = await _httpClient.GetAsync("/auth");
        
        Assert.NotNull(response);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}