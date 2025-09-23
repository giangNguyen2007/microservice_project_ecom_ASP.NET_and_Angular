using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Dtos.Comment;

public class GetCommentDto
{
    public Guid Id { get; set; }

    [Required]
    public string Content { get; set; }

    public Guid ProductId { get; set; }
    
    public Guid? userId { get; set; }
    
    public int? Rating { get; set; } 
}
