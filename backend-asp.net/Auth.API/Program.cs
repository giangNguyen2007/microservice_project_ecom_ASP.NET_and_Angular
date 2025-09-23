using Auth.API.GrpcServices;
using Auth.API.Model;
using Auth.API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.JwtSetting;

var builder = WebApplication.CreateBuilder(args);

// ===== Logger ====
builder.Logging.ClearProviders();  // Clear default logging providers
builder.Logging.AddConsole();


// ==== register Database MySQL ======
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


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// create the database if not exists
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
db.Database.EnsureCreated();



app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication(); // check 
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcUserInfoServer>(); // New gRPC service

app.Run();

namespace Auth.API
{
    public partial class AuthApiProgram {}
}