namespace SharedLibrary.MassTransit.RabbitMQ;

public class PaymentResultMsg
{
    public Guid OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}