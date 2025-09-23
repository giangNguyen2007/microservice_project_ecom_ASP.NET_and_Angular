using GateWay.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HelloTest.Test.Gateway.API;

public class GateWayTestWebAppFactory : WebApplicationFactory<GateWayProgram>
{
    private readonly string _mockUrl;

    public GateWayTestWebAppFactory(string mockUrl)
    {
        _mockUrl = mockUrl;
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace configuration or injected HttpClient base address
            // services.PostConfigure<GatewayOptions>(opts =>
            // {
            //     opts.DownstreamBaseUrl = _mockUrl;
            // });
        });
    }
}