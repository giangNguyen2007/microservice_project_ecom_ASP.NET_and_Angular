using MassTransit;
using Order.API.Dtos;
using Order.API.GRPC;
using Order.API.Interfaces;
using Order.API.Services;
using SharedLibrary;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Order.API.RabbitMqService;

public class MtPaymentResultConsumer : IConsumer<PaymentResultMsg>
{
    private PaymentResultHandlerService _paymentResultHandlerService;

    public MtPaymentResultConsumer(PaymentResultHandlerService paymentResultHandlerService)
    {
        _paymentResultHandlerService = paymentResultHandlerService;
    }

    public async Task Consume(ConsumeContext<PaymentResultMsg> context)
    {
        var paymentResult = context.Message;
        // Your business logic here
        Console.WriteLine($"Received Paymennt Message: {paymentResult.OrderId}");

        await _paymentResultHandlerService.HandlePaymentAsync(paymentResult);
     
        
       await Task.CompletedTask;
       
    }
}
