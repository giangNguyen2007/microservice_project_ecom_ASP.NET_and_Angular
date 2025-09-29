using System.Reflection.Metadata.Ecma335;
using System.Text;
using ProductAPI.Data;
using ProductAPI.Interfaces;
using ProductAPI.Repository;
using ProductAPI.Dtos;
using ProductAPI.GRPC.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProductAPI.Config;
using ProductAPI.Middleware;
using ProductAPI.Services;
using SharedLibrary.JwtSetting;
using SharedLibrary.MassTransit.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


// set Kestrel to support HTTP2
builder.WebHost.ConfigureKestrel(o =>
{
    // http1 => for rest api
    // http2 plain text => for grpc, without ssl
    o.ListenAnyIP(5000, lo => lo.Protocols = HttpProtocols.Http1);
    o.ListenAnyIP(5001, lo => lo.Protocols = HttpProtocols.Http2);
});

// ===== Logger ====
builder.Logging.ClearProviders();  // Clear default logging providers
builder.Logging.AddConsole();

builder.Services.AddControllers();


// extract dbConfig 
var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection");

System.Console.WriteLine(sqliteConnectionString);
System.Console.WriteLine(sqliteConnectionString);

builder.Services.AddDbContext<ProductDBContext>(option =>
{
    // option.UseMySql(
    //     dbConfig.ConnectionString,
    //     ServerVersion.AutoDetect(dbConfig.ConnectionString)
    // );
    
    option.UseSqlite(sqliteConnectionString);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<ProductStockService>();

// =============== GRPC ============
builder.Services.AddGrpc();
builder.Services.AddScoped<GrpcProductServer>(); // register directly the server to DI Container
builder.Services.AddGrpcReflection();


// =============== SECURITY ============

builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // register Registers the authentication middleware in the DI container.
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(JwtSetting.Key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin"); 
    });
});

// ================ Cors ====================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// =============== ===== ===========

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// create the database if not exists
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ProductDBContext>();
db.Database.EnsureCreated();

// removed because using http1 and http2
// app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

// app.UseAuthentication(); // check 
// app.UseAuthorization();

app.UseMiddleware<UserDataMiddleware>();

app.MapControllers();
app.MapGrpcService<GrpcProductServer>(); // New gRPC service

app.MapGrpcReflectionService();


app.Run();

namespace ProductAPI
{
    public partial class ProductApiProgram {}
}