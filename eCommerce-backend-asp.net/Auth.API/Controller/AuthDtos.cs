namespace Auth.API.Controller;

public record RegisterDto(string email, string password);

public record LoginDto(string email, string password);
