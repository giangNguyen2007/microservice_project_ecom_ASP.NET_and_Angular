using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.API.Config;
using Payment.API.data;
using Payment.API.RabbitMqServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOptions<ApiConfig>()
    .Bind(builder.Configuration.GetSection("ApiConfig"))
    .ValidateDataAnnotations(); 

// check
var apiConfig = builder.Configuration.GetSection("ApiConfig").Get<ApiConfig>();

System.Console.WriteLine("BankAppUrl: " + apiConfig.BankAppUrl);
System.Console.WriteLine("GatewayUrl: " + apiConfig.GatewayUrl);

// ===== Logger ====
builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();


builder.Services.AddDbContext<PaymentRequestDbContext>(option =>
{
    option.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
});

builder.Services.AddControllers();



var rabbitMqUrl = builder.Configuration["RabbitMqUrl"];

System.Console.WriteLine("mq= " + rabbitMqUrl);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentRequestConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqUrl), "/", h =>
        {
            h.Username("guest"); 
            h.Password("guest");
        });
        
        // bind consumer to endpoint
        cfg.ReceiveEndpoint("payment-request-queue", e =>
            e.ConfigureConsumer<PaymentRequestConsumer>(context)
        );
       
    });
    
});

builder.Services.AddScoped<PaymentResultPublisher>();

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


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();


// create the database if not exists
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<PaymentRequestDbContext>();
db.Database.EnsureCreated();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

//app.UseHttpsRedirection();

app.UseCors();



app.MapControllers();

app.Run();

namespace Payment.API
{
    public partial class PaymentApiProgram {}
}