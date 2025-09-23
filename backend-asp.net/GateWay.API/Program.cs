using System.Text.Json;
using GateWay.API.Config;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);



var downstreamApi = builder.Configuration.GetSection("DownstreamApi").Get<DownstreamApi>();

var gatewayConfig = builder.Configuration.GetSection("GatewayConfig").Get<GatewayConfig>();

System.Console.WriteLine($"DownstreamApi: {JsonSerializer.Serialize(downstreamApi)}");



var ocelotConfig = new
{
    Routes = new[]
    {
        new {
            DownstreamPathTemplate = "/product",
            DownstreamScheme = downstreamApi.ProductApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.ProductApiHost, Port = downstreamApi.ProductApiPort } },
            UpstreamPathTemplate = "/product",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true   // to disable SSL certificate validation
            }
        },
        
        new {
            DownstreamPathTemplate = "/auth/{catchAll}",
            DownstreamScheme = downstreamApi.AuthApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.AuthApiHost, Port = downstreamApi.AuthApiPort } },
            UpstreamPathTemplate = "/auth/{catchAll}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true   
            }
        },
        
        new {
            DownstreamPathTemplate = "/product/{catchAll}",
            DownstreamScheme = downstreamApi.ProductApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.ProductApiHost, Port = downstreamApi.ProductApiPort } },
            UpstreamPathTemplate = "/product/{catchAll}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true  
            }
        },
        
        new {
            DownstreamPathTemplate = "/order/{catchAll}",
            DownstreamScheme = downstreamApi.OrderApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.OrderApiHost, Port = downstreamApi.OrderApiPort} },
            UpstreamPathTemplate = "/order/{catchAll}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true  
            }
        },
        
        new {
            DownstreamPathTemplate = "/order",
            DownstreamScheme = downstreamApi.OrderApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.OrderApiHost, Port = downstreamApi.OrderApiPort} },
            UpstreamPathTemplate = "/order",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true  
            }
        },
        
        new {
            DownstreamPathTemplate = "/payment/{catchAll}",
            DownstreamScheme = downstreamApi.PaymentApiScheme,
            DownstreamHostAndPorts = new[] { new { Host = downstreamApi.PaymentApiHost, Port = downstreamApi.PaymentApiPort } },
            UpstreamPathTemplate = "/payment/{catchAll}",
            UpstreamHttpMethod = new[] { "GET", "PUT", "POST", "DELETE" },
            HttpHandlerOptions = new {
                DangerousAcceptAnyServerCertificateValidator = true  
            }
        }
        
        
    },
    GlobalConfiguration = new { BaseUrl = gatewayConfig.Url }
};


// Serialize to JSON
var ocelotJson = JsonSerializer.Serialize(ocelotConfig);

var ocelotConfigStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ocelotJson));

builder.Configuration.AddJsonStream(ocelotConfigStream);

Console.WriteLine(
    $"Bypass TLS? {builder.Configuration["Routes:0:HttpHandlerOptions:DangerousAcceptAnyServerCertificateValidator"]}"
);

Console.WriteLine(
    $"Bypass TLS? {builder.Configuration["Routes:1:HttpHandlerOptions:DangerousAcceptAnyServerCertificateValidator"]}"
);

Console.WriteLine(
    $"Bypass TLS? {builder.Configuration["Routes:2:HttpHandlerOptions:DangerousAcceptAnyServerCertificateValidator"]}"
);

// builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot()
 
    ;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

await app.UseOcelot();

app.Run();

namespace GateWay.API
{
    public partial class GateWayProgram {}
}