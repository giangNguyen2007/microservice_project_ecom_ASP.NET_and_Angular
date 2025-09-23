using System.Net.Http.Json;
using Xunit.Abstractions;

namespace HelloTest.Test.Http;

public class MyHttpTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private HttpClient httpClient = new HttpClient();
    
    
    
    public MyHttpTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public async Task Test1()
    {

        // simulate payment processing delay
        var transactionRequest = new
        {
            userId = "71a9e301-a9f4-4a52-9f9e-464993c83248",
            accountId = "6de940f3-5fa3-43ec-9663-8c24318effa2" ,
            amount = 100,
            
            // represent the account of the ecommerce company, which is external to the bank app
            // can be fictive value 
            destinationAccountId =  "6de940f3-5fa3-43ec-9663-8c24318effa1", 
            
            description = $"Payment request from ASP.NET Core Microservice",
            
            sourceUrl = "http://localhost:5160/payment/bank-notification" + "/" + "6de940f3-5fa3-43ec-9663-8c24318effa2"
        };

        var response = await httpClient.PostAsJsonAsync("http://localhost:8085/incoming-transaction", transactionRequest);

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        var responseBody = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(responseBody);

    }
}