using System.ComponentModel.DataAnnotations;

namespace Order.API.Config;

public class DbConfig
{
    [Required]
    public string Host { get; set; }
    [Required]
    public int Port { get; set; }
    [Required]
    public string User { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Database { get; set; }
    
    public string ConnectionString => $"Server={Host};Port={Port};Database={Database};User={User};Password={Password};";
}