using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.API.Config;
using Order.API.Data;
using Order.API.GRPC;
using Order.API.Interfaces;
using Order.API.Middleware;
using Order.API.RabbitMqService;
using Order.API.Repository;
using Order.API.Services;
using Product;
using SharedLibrary.JwtSetting;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
System.Console.WriteLine("allow unencrypted grpc calls");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();  // Clear default logging providers
builder.Logging.AddConsole();



var dbConfig = builder.Configuration.GetSection("DbConfig").Get<DbConfig>();

System.Console.WriteLine(dbConfig.ConnectionString);

builder.Services.AddDbContext<OrderDBContext>(option =>
{
    option.UseMySql(
        dbConfig.ConnectionString,
        ServerVersion.AutoDetect(dbConfig.ConnectionString)
    );
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<PaymentResultHandlerService>();
builder.Services.AddScoped<OrderService>();


// ================ GRPC ===============
// allow unencrypted grpc calls



var grpcUrl = builder.Configuration["GrpcUrl"];

System.Console.WriteLine("GrpcUrl: " + grpcUrl);

builder.Services.AddGrpcClient<ProductService.ProductServiceClient>(o =>
{
    o.Address = new Uri(grpcUrl);
});

builder.Services.AddScoped<GrpcClientService>();


// ================ MASS TRANSIT ========================================

var rabbitMqUrl = builder.Configuration["RabbitMqUrl"];

System.Console.WriteLine("RabbitMqUrl: " + rabbitMqUrl);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MtPaymentResultConsumer>();
    //x.AddConsumer<MassTransitConsumer2>(); // Register the consumer

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqUrl, h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Configure the receive endpoint and bind the consumer
        cfg.ReceiveEndpoint("order-submitted-queue", e =>
        {
            e.ConfigureConsumer<MtPaymentResultConsumer>(context);
        });
        
     
    });
});

builder.Services.AddScoped<MtPaymentRequestPublisher>();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<OrderDBContext>();
db.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserDataMiddleware>();

app.MapControllers();

app.Run();

namespace Order.API
{
    public partial class OrderApiProgram {}
}