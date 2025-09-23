using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Model;

public class CommentModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? userId { get; set; }
    
    [ForeignKey("ProductModel")]
    [Required(ErrorMessage = "ProductId is mandatory.")]
    public Guid ProductId { get; set; }
    
    [Required(ErrorMessage = "Comment text is mandatory.")]
    public string Content { get; set; } = string.Empty;
    
    public int? Rating { get; set; } 
    
}
