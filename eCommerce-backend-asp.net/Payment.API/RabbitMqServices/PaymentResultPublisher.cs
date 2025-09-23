using MassTransit;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Payment.API.RabbitMqServices;

public class PaymentResultPublisher
{
    private IPublishEndpoint _publishEndpoint;

    public PaymentResultPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPaymentResult(Guid orderId, bool success)
    {
        // send back payment result 
        await _publishEndpoint.Publish<PaymentResultMsg>(new PaymentResultMsg()
            {
                OrderId =  orderId,
                PaymentId = Guid.NewGuid(),
                Success = success
            }
        );
    }
}