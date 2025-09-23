using MassTransit;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Order.API.RabbitMqService;

// MassTransit Publisher => send payment event 
// to be processed by PaymentAPI microservice
public class MtPaymentRequestPublisher
{
    private IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MtPaymentRequestPublisher> _logger;

    public MtPaymentRequestPublisher(IPublishEndpoint publishEndpoint, ILogger<MtPaymentRequestPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishPayment(Guid orderId, int amount, Guid bankUserId, Guid bankAccountId)
    {
        
        _logger.LogInformation($"[Order.API] Publishing Payment Request for OrderId : {orderId} Amount : {amount}");
        
        
        // create message
        
        await _publishEndpoint.Publish(new PaymentRequestMsg()
        {
            OrderId = orderId,
            Amount = amount,
            BankAccountId = bankAccountId,
            UserId = bankUserId
        });
    }
}