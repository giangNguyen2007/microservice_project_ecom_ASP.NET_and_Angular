using System.ComponentModel.DataAnnotations;

namespace Order.API.Dtos;

public class PutOrderDto
{
    

    public int? Quantity { get; set; }
    
 
    public String? OrderStatus { get; set; }
}