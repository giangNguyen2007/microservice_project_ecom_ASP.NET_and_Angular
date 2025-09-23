using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payment.API.data;
using Payment.API.RabbitMqServices;

namespace Payment.API.Controller;


// create a record of product
public record BankResponse(bool success);


[Route("/payment")]
[ApiController]
public class PaymentController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly ILogger<PaymentController> _logger;
    private readonly PaymentRequestDbContext _db;
    
    private readonly PaymentResultPublisher _paymentResultPublisher; // MassTransit Publisher
    
    public PaymentController(ILogger<PaymentController> logger, PaymentRequestDbContext db, PaymentResultPublisher paymentResultPublisher)
    {
        _logger = logger;
        _db = db;
        _paymentResultPublisher = paymentResultPublisher;
    }
    
    // endpoint to receive bank notification about transaction processing 's result
    // role : serving as relay between Bank and OrderAPI
    // path : host:port/payment/notification
    [HttpPost("bank-notification/{orderId}")]
    public async Task<IActionResult> ReceiveBankNotification( [FromRoute] Guid orderId, [FromQuery] bool success)
    {
        
        if (orderId == Guid.Empty)
        {
            _logger.LogError("Invalid orderId in bank notification");
            return BadRequest("Invalid orderId");
        }
        
        
        _logger.LogInformation("Received bank notification for Order " + orderId + " with payment result = " + success);
        
        //extract payment info from db
        var paymentRequest = await _db.PaymentRequests.FindAsync(orderId);
        if (paymentRequest == null)
        {
            _logger.LogError($"Payment request with OrderId {orderId} not found.");
            return NotFound($"Payment request with OrderId {orderId} not found.");
        }
        
         // publish payment result to OrderAPI
        await _paymentResultPublisher.PublishPaymentResult(orderId, success);
        return Ok();
    }
}