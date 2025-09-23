using System.ComponentModel.DataAnnotations;
using ProductAPI.Dtos.Comment;
using ProductAPI.Model;

namespace ProductAPI.Dtos.Game;

public record class GetProductDto
{
    public Guid Id { get; set; }
    
    [Required]
    public  string Title { get; set; }
    
    public  string? Description { get; set; }
    
    public  string? PhotoUrl { get; set; }
    
    public string? Category { get; set; }
    
    public  int Price { get; set; }
    
    public  int Stock { get; set; }

    public List<CommentModel> Comments { get; set; }
};
