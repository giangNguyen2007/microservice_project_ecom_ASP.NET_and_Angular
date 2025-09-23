namespace ProductAPI.Dtos.Game;

public class PutProductDto
{
    
    public  string? Title { get; set; }
    
    public  string? Description { get; set; }
    
    public  string? PhotoUrl { get; set; }
    
    public string? Category { get; set; }
    
    public  int? Price { get; set; }
    
    public  int? Stock { get; set; }
}
