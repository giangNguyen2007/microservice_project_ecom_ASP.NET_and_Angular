using System.ComponentModel.DataAnnotations;

namespace Order.API.Data;

public class OrderModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public String CustomerEmail { get; set; }
    
    public DateTime OrderDate { get; set; } =  DateTime.Now;
    
    [Required]
    public String OrderStatus { get; set; }
    
    [Required]
    public ICollection<SingleItemOrder> OrderItemList { get; set; } = new List<SingleItemOrder>();
}