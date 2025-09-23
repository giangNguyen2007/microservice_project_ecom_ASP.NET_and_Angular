namespace SharedLibrary.MassTransit.RabbitMQ;

public class PaymentRequestMsg
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
    
    // payment info
    // simulation : userId, bank accountId
    
    public Guid BankAccountId { get; set; }
    
    public Guid UserId { get; set; }
}