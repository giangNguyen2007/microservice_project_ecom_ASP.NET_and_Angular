using System.Security.Claims;

namespace ProductAPI.Middleware;

public class UserDataMiddleware
{
    private readonly RequestDelegate _next;
    
    
    public UserDataMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // context.Users.Identity is populated by the authentication middleware
        // context.User.Identity contains claims from the JWT token
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // extract user info from claims
            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            
            
            // If claims are missing, respond with 401 Unauthorized
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userRole))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing required user email / role.");
                return;
            }
            
            
            // For debugging purposes
            Console.WriteLine($"UserDataMiddleware: Extracted UserEmail = {userEmail}, UserRole = {userRole}");
            
            // Attach to HttpContext.Items for downstream access
            context.Items["UserEmail"] = userEmail;
            context.Items["UserRole"] = userRole;
        }

        await _next(context);
    }
    
    
}