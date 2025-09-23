using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Model;

public class ProductModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Title is mandatory.")]
    public  string Title { get; set; }
    
    public  string? Description { get; set; }
    
    public  string? PhotoUrl { get; set; }
    
    public string? Category { get; set; }
    
    [Required(ErrorMessage = "Price is mandatory.")]
    public  int Price { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is mandatory.")]
    public  int Stock { get; set; }
    
    public int ReservedStock { get; set; } = 0;

    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
}