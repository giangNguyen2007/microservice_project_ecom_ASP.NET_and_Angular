using Auth.API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.JwtSetting;

namespace Auth.API.Service;

public class CustomStaticRegistrationService
{
    public static void registerDbContext(WebApplicationBuilder builder)
    {
        // var dbHost = builder.Configuration["DB_HOST"];
        // var dbPort = builder.Configuration["DB_PORT"];
        // var dbUser = builder.Configuration["DB_USER"];
        // var dbPassword = builder.Configuration["DB_PASSWORD"];
        // var dbName = builder.Configuration["DB_NAME"];
        //
        // var connectionString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";


        builder.Services.AddDbContext<UserDbContext>(option =>
        {
            option.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
        });
    }

    public static void registerAuthenticationhAndAuthorisation(WebApplicationBuilder builder)
    {
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
    }
}