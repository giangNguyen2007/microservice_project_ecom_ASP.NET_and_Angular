namespace SharedLibrary.MassTransit.RabbitMQ;

public class MqMessage
{
    public string Message { get; set; }
    public int Price { get; set; }
}

public class UpdateStockMsg
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}