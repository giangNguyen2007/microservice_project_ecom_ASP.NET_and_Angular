using Auth.API.GrpcServices;
using Auth.API.Model;
using Auth.API.Service;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// ===== Logger ====
builder.Logging.ClearProviders();  // Clear default logging providers
builder.Logging.AddConsole();


// ==== register Database ======
CustomStaticRegistrationService.registerDbContext(builder);

builder.Services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

builder.Services.AddControllers();


// ==== GRPC ======
builder.Services.AddGrpc();
builder.Services.AddScoped<GrpcUserInfoServer>(); // register directly the server to DI Container

// ===== JWT Authentication ======
CustomStaticRegistrationService.registerAuthenticationhAndAuthorisation(builder);


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



// ================ app Config ====================

var app = builder.Build();

// create the database if not exists
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
db.Database.EnsureCreated();



app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcUserInfoServer>(); 

app.Run();

namespace Auth.API
{
    public partial class AuthApiProgram {}
}