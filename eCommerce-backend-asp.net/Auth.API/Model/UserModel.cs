using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Model;

[Index(nameof(Email), IsUnique  = true)]
public class UserModel
{
    [Key]
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public required string Role { get; set; }

}