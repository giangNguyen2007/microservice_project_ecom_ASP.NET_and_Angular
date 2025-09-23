using MassTransit;
using SharedLibrary.MassTransit.RabbitMQ;

namespace HelloTest.Test.Order.API.MassTransitConsumer;

// Send Message to running RabbitMQ container
// required : Running RabbitMQ, app running on local machine
// check result by looking at app console

public class TestByBusControl
{
    [Fact]
    public async Task SendPaymentResultMessage()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq( cfg => 
        {
            cfg.Host("rabbitmq://localhost", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });
    
        await bus.StartAsync();
    
        try
        {
            var endpoint = await bus.GetSendEndpoint(new Uri("queue:order-submitted-queue"));
       

            await endpoint.Send(new PaymentResultMsg
            {
                OrderId = Guid.Parse( "0fb7b55e-afb7-42e4-b3b3-c399ddf1a0b8"),
                PaymentId = Guid.NewGuid(),
                Success = true
                
            });
        
        }
        finally
        {
            await bus.StopAsync();
        }
    }
    
    [Fact]
    public async Task SendFailurePaymentResultMessage()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq( cfg => 
        {
            cfg.Host("rabbitmq://localhost", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });
    
        await bus.StartAsync();
    
        try
        {
            var endpoint = await bus.GetSendEndpoint(new Uri("queue:order-submitted-queue"));
       

            await endpoint.Send(new PaymentResultMsg
            {
                OrderId = Guid.Parse( "0fb7b55e-afb7-42e4-b3b3-c399ddf1a0b8"),
                PaymentId = Guid.NewGuid(),
                Success = false
                
            });
        
        }
        finally
        {
            await bus.StopAsync();
        }
    }
}
