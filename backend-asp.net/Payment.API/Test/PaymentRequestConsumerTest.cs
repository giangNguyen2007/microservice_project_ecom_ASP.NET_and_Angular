using Payment.API.data;
using Xunit;
using Xunit.Abstractions;

namespace Payment.API.RabbitMqServices;

public class PaymentRequestConsumerTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private HttpClient httpClient = new HttpClient();
    
    private PaymentRequestDbContext _dbContext;
    
    // private PaymentRequestConsumer _consumer = new PaymentRequestConsumer();
    
    public PaymentRequestConsumerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    
    [Fact]
    public async Task Test1()
    {
   
        // simulate payment processing delay
        var transactionRequest = new
        {
            userId =  "71a9e301-a9f4-4a52-9f9e-464993c83248",
            accountId = "6de940f3-5fa3-43ec-9663-8c24318effa2",
            destinationAccountId =  "6de940f3-5fa3-43ec-9663-8c24318effa1",
            amount = 100,
            description = $"Payment from ASP.NET Core Microservice",
            notificationUrl = "http://localhost:5160/payment/payment-notification"
        };
    
        var response = await httpClient.PostAsJsonAsync("http://localhost:8085/incoming-transaction", transactionRequest);
      
        _testOutputHelper.WriteLine(response.StatusCode.ToString());
        
        var responseBody = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(responseBody);
        
    }

    [Fact]
    public async Task TestDbContext()
    {
        var myEntity = new PaymentRequestModel
        {
            OrderId = Guid.NewGuid(),
            Amount = 100,
            BankAccountId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        
        _dbContext.PaymentRequests.Add(myEntity);
        await _dbContext.SaveChangesAsync();
        
        var retrievedEntity = await _dbContext.PaymentRequests.FindAsync(myEntity.OrderId);
        Assert.NotNull(retrievedEntity);
        _testOutputHelper.WriteLine(retrievedEntity.ToString());
    }
    
    
}