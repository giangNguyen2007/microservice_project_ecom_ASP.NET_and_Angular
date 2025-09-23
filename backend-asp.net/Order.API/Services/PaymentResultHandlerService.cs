using Order.API.Dtos;
using Order.API.GRPC;
using Order.API.Interfaces;
using Product;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Order.API.Services;

public class PaymentResultHandlerService
{
    private IOrderRepository _orderRepository;
    private GrpcClientService _grpcClient;

    public PaymentResultHandlerService(IOrderRepository orderRepository, GrpcClientService grpcClient)
    {
        _orderRepository = orderRepository;
        _grpcClient = grpcClient;
    }

    public async Task HandlePaymentAsync(PaymentResultMsg paymentResult)
    {
        try
        {
            // retrieve order info
            var myOrder = await _orderRepository.getByIdAsync(paymentResult.OrderId);
                 
            Console.WriteLine($"Retrieve order Info : {myOrder?.Id}");

            if (paymentResult.Success)
            {
                
                // send stock update to ProductAPI
                StockUpdateReply reply = await _grpcClient.SendStockUpdateRequest(myOrder);
                
                // handle stock update failure
                // to be done later ...
                
                // notify user
                
                // update order status to confirmed
                await _orderRepository.updateOrderStatusAsync(paymentResult.OrderId, new PutOrderDto
                {
                    OrderStatus = "Confirmed"
                });
                
                Console.WriteLine($"Complete update orderstatus : {myOrder?.Id}");
                
            }
            
            // if PAYMENT FAILED
            else
            {
                // update order status to failed
                await _orderRepository.updateOrderStatusAsync(paymentResult.OrderId, new PutOrderDto
                {
                    OrderStatus = "Payment Failed"
                });
                
                // restore stock from Reservation
                // send stock release to ProductAPI
                
                await _grpcClient.SendStockReleaseRequest(myOrder);
                
                
                // notify user
                
                Console.WriteLine($"Complete update orderstatus : Failed {myOrder?.Id}");
            }

            
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}