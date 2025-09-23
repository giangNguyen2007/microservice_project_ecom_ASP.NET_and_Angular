using Order.API.Data;
using Order.API.Dtos;

namespace Order.API.Interfaces;

public interface IOrderRepository
{
    Task<List<OrderModel>> getAllAsync();
    
    Task<List<OrderModel>> getByUserEmailAsync(String userEmail);

    
    Task<OrderModel> getByIdAsync(Guid id);
    Task<OrderModel> createAsync(String userEmail, PostOrderDto postOrderDto);

    Task<OrderModel> updateOrderStatusAsync(Guid id, PutOrderDto putOrderDto);
    Task<OrderModel?> deleteAsync(Guid id);

}