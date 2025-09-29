using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace Order.API.Dtos;

public class PostOrderDto
{

    [Required] public List<OrderItemDto> Items { get; set; }

    [Required] public int MontantTotal { get; set; }
    
    
    //payment info
    // userId, bank accountId
    // to be sent to payment microservice
    [Required] public Guid BankUserId { get; set; }
    
    [Required] public Guid BankAccountId { get; set; }
}



public class OrderItemDto
{
    [Required]
    
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}