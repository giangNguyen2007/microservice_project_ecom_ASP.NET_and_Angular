using System.ComponentModel.DataAnnotations;

namespace Payment.API.data;

public class PaymentRequestModel
{
    [Key]
    public Guid OrderId { get; set; }
    
    public int Amount { get; set; }
    
    public Guid BankAccountId { get; set; }
    
    public Guid UserId { get; set; }
}