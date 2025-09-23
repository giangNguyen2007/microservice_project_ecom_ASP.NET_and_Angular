using Microsoft.EntityFrameworkCore;
using Order.API.Data;
using Order.API.Dtos;
using Order.API.Interfaces;

namespace Order.API.Repository;

public class OrderRepository : IOrderRepository
{
    private OrderDBContext _orderDBContext;
    
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrderDBContext orderDbContext)
    {
        _orderDBContext = orderDbContext;
    }

    public async Task<List<OrderModel>> getAllAsync()
    {
        var orders = await _orderDBContext.Orders
            .Include( c => c.OrderItemList)
            .OrderByDescending(c => c.OrderDate)
            .ToListAsync();
        return orders;
    }

    public Task<List<OrderModel>> getByUserEmailAsync(string userEmail)
    {
        var orders = _orderDBContext.Orders
            .Where(c => c.CustomerEmail == userEmail)
            .Include( c => c.OrderItemList)
            .OrderByDescending(c => c.OrderDate)
            .ToListAsync();
        return orders;
    }

    public async Task<OrderModel> getByIdAsync(Guid id)
    {
        var order = await _orderDBContext.Orders
            .Include( c => c.OrderItemList)
            
            .FirstOrDefaultAsync(c => c.Id == id);
        if (order == null)
            throw new KeyNotFoundException("Order not found");
        
        return order;
    }

    public async Task<OrderModel> createAsync(String userEmail, PostOrderDto postOrderDto)
    {
        List<SingleItemOrder> orderItems = postOrderDto.Items.Select(item => new SingleItemOrder
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
        }).ToList();
        
        
        OrderModel newOrder = new OrderModel
        {
            Id = Guid.NewGuid(),
            CustomerEmail = userEmail,
            OrderDate = DateTime.Now,
            OrderItemList = orderItems,
            OrderStatus = "Pending Payment"
        };
        
        await _orderDBContext.Orders.AddAsync(newOrder);
        await _orderDBContext.SaveChangesAsync();
        
        return newOrder;
    }

    public async Task<OrderModel> updateOrderStatusAsync(Guid id, PutOrderDto putOrderDto)
    {
        var order = await _orderDBContext.Orders.FirstOrDefaultAsync(g => g.Id == id);
        if (order == null)
            throw new KeyNotFoundException("Order not found");
        
        
        if (putOrderDto.OrderStatus != null){ order.OrderStatus = putOrderDto.OrderStatus; }

        await _orderDBContext.SaveChangesAsync();
        
        return order;
        
    }

    public async Task<OrderModel?> deleteAsync(Guid id)
    {
        
        var order = await _orderDBContext.Orders.FirstOrDefaultAsync(g => g.Id == id);
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found");
        }

        _orderDBContext.Remove(order);
        await _orderDBContext.SaveChangesAsync();
        
        return order;
    }
}