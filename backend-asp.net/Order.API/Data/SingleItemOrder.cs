using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Order.API.Data;

public class SingleItemOrder
{
    
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ForeignKey("OrderModel")]
    [Required(ErrorMessage = "ProductId is mandatory.")]
    public Guid ParentId { get; set; }
    
    
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [JsonIgnore]
    public virtual OrderModel Parent { get; set; }
}