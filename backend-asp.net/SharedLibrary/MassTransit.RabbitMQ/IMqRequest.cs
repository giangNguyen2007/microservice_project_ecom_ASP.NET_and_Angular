namespace SharedLibrary.MassTransit.RabbitMQ;

public interface IMqRequest
{
    int Id { get; set; }
}

public interface IMqResponse
{
    int Id { get; set; }
    string Message { get; set; }
}