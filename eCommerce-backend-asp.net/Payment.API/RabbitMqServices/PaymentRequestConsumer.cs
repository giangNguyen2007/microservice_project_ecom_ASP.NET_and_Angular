using MassTransit;
using Microsoft.Extensions.Options;
using Payment.API.Config;
using Payment.API.data;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Payment.API.RabbitMqServices;

public class PaymentRequestConsumer : IConsumer<PaymentRequestMsg>
{
    private HttpClient httpClient = new HttpClient();
    private PaymentRequestDbContext _paymentRequestDbContext;
    
    private ApiConfig _apiConfig;
    
    //add logger below
    private readonly ILogger<PaymentRequestConsumer> _logger;

    public PaymentRequestConsumer(PaymentRequestDbContext paymentRequestDbContext, IOptions<ApiConfig> options, ILogger<PaymentRequestConsumer> logger)
    {
        _paymentRequestDbContext = paymentRequestDbContext;
        _logger = logger;
        _apiConfig = options.Value;
        
        _logger.LogInformation(" config value: " + _apiConfig.BankAppUrl);
        _logger.LogInformation(" config value: " + _apiConfig.GatewayUrl);
        
    }

    public async Task Consume(ConsumeContext<PaymentRequestMsg> context)
    {
        var paymentRequestMsg = context.Message;
        
        Console.WriteLine($"PaymentService: Receive payment request: Order id = {paymentRequestMsg.OrderId}, amount = {paymentRequestMsg.Amount}");
        
        // save request to db
        var paymentRequest = new PaymentRequestModel
        {
            OrderId = paymentRequestMsg.OrderId,
            Amount = paymentRequestMsg.Amount,
            BankAccountId = paymentRequestMsg.BankAccountId,
            UserId = paymentRequestMsg.UserId 
        };
        
        _paymentRequestDbContext.PaymentRequests.Add(paymentRequest);
        await _paymentRequestDbContext.SaveChangesAsync();
        
        await SendRequestToBankApp( paymentRequestMsg.UserId, paymentRequestMsg.BankAccountId, paymentRequestMsg.Amount, paymentRequestMsg.OrderId );

    }

    public async Task SendRequestToBankApp( Guid userId, Guid accountId, int amount, Guid orderId )
    {
        // simulate payment processing delay
        
        var transactionRequest = new
        {
            userId,
            accountId,
            amount,
            
            // represent the account of the ecommerce company, which is external to the bank app
            // can be fictive value 
            destinationAccountId =  "6de940f3-5fa3-43ec-9663-8c24318effa1", 
            
            description = $"Payment request from ASP.NET Core Microservice",
            
            sourceUrl = _apiConfig.GatewayUrl+ "/payment/bank-notification" + "/" + orderId.ToString()
        };
        
        var response = await httpClient.PostAsJsonAsync(_apiConfig.BankAppUrl +"/incoming-transaction", transactionRequest);
    }
    
}