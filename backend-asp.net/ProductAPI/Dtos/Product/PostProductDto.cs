using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Dtos.Game;

public class PostProductDto
{
    
    [Required]
    public  string Title { get; set; }
    
    public  string? Description { get; set; }
    
    public  string? PhotoUrl { get; set; }
    
    public string? Category { get; set; }
    
    public  int Price { get; set; }
    
    public  int Stock { get; set; }
    
}

