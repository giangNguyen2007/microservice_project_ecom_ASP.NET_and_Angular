using Order.API.Controller;
using Order.API.Data;
using Order.API.Dtos;
using Order.API.GRPC;
using Order.API.Interfaces;
using Order.API.RabbitMqService;

namespace Order.API.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepo;
    
    private readonly GrpcClientService _grpcClientService;

    private MtPaymentRequestPublisher _mtPaymentRequestPublisher;  // MassTransit Publisher
    
    private readonly ILogger<OrderController> _logger;

    public OrderService(IOrderRepository orderRepo, GrpcClientService grpcClientService, MtPaymentRequestPublisher mtPaymentRequestPublisher, ILogger<OrderController> logger)
    {
        _orderRepo = orderRepo;
        _grpcClientService = grpcClientService;
        _mtPaymentRequestPublisher = mtPaymentRequestPublisher;
        _logger = logger;
    }
    
    public async Task<List<OrderModel>> GetAllOrdersAsync()
    {
        return await _orderRepo.getAllAsync();
    }
    
    public async Task<List<OrderModel>> GetOrdersByUserEmailAsync(String userEmail)
    {
        var results = await _orderRepo.getByUserEmailAsync(userEmail);
        
        if (results == null)
            throw new KeyNotFoundException("No orders found for this user");

        return results;
    }
    
    public async Task<OrderModel> GetOrderByIdAsync(Guid id)
    {
        var results = await _orderRepo.getByIdAsync(id);
        
        if (results == null)
            throw new KeyNotFoundException("No orders found for this id");

        return results;
    }
    
    // Method to handle order creation and initiate payment
    public async Task<OrderModel> CreateOrderAsync(String userEmail, PostOrderDto postOrderDto)
    {
        _logger.LogInformation("Creating new order for user: {UserEmail}", userEmail);
        
        // check that order contains at least one item
        if ( postOrderDto.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item");
        
        // send gRPC request to Product API to check stock + valid product ids
        //  and reserve stock if available
        var reservationReply = await _grpcClientService.SendReservationRequest(postOrderDto);

        //  if not enough stock for at least one product, throw exception
        if (!reservationReply.Success)
        {
            _logger.LogError($"Reservation failed: {reservationReply.ErrorMessage}");
            throw new InvalidOperationException($"Reservation failed: {reservationReply.ErrorMessage}");
        }
         
        // create order with status "Pending Payment"
        OrderModel createdOrder = await _orderRepo.createAsync(userEmail, postOrderDto);
        
        
        // send payment event, including payment info, to payment service
        await _mtPaymentRequestPublisher.PublishPayment(
            createdOrder.Id, postOrderDto.MontantTotal, postOrderDto.BankUserId, postOrderDto.BankAccountId
        );
        
        return createdOrder;
        
    }
}