using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.API.Model;
using Auth.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.API.Controller;

[Route("/auth")]
[ApiController]
[Authorize]  // all endpoints in this controller require authentication unless [AllowAnonymous] is specified
public class AuthController : Microsoft.AspNetCore.Mvc.Controller
{
    private UserDbContext _userDbContext;
    private IPasswordHasher<UserModel> _passwordHasher;
    private IConfiguration _config;
    
    private ILogger<AuthController> _logger;

    public AuthController(UserDbContext userDbContext, IPasswordHasher<UserModel> passwordHasher, IConfiguration config, ILogger<AuthController> logger)
    {
        _userDbContext=  userDbContext;
        _passwordHasher = passwordHasher;
        _config = config;
        _logger = logger;
        ;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetALlUsers()
    {
        List<UserModel> allUsers = await _userDbContext.Users.ToListAsync();
        return Ok(allUsers);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (await _userDbContext.Users.AnyAsync(u => u.Email == registerDto.email))
            return BadRequest(new { errorMessage = "Email is already registered."} );

        var newUser = new UserModel
        {
            Email = registerDto.email,
            Password = registerDto.password,
            Role = "user"
        };
        
        await _userDbContext.Users.AddAsync(newUser);
        await _userDbContext.SaveChangesAsync();
        
        _logger.LogInformation("New user registered: {Email}", newUser.Email);
        
        return Ok( new
        {
            message = "User registered successfully",
            email = newUser.Email,
            token = StaticJwtTokenService.GenerateJwtToken(newUser),
            role = newUser.Role
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.email);

        if (user == null)
        {
            return Unauthorized(new {errorMessage = "Invalid credentials - unregistered email"});
        }

        if (loginDto.password != user.Password)
            return Unauthorized(new {errorMessage = "Invalid credentials - wrong password"});
        
        _logger.LogInformation("User logged in: {Email}", user.Email);

        return Ok( new
        {
            message = "login sucessful",
            email = user.Email,
            token = StaticJwtTokenService.GenerateJwtToken(user),
            role = user.Role
        });
    }
   
   
}