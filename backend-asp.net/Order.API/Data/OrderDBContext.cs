using Microsoft.EntityFrameworkCore;

namespace Order.API.Data;

public class OrderDBContext : DbContext
{
    public OrderDBContext(DbContextOptions<OrderDBContext> options) : base(options)
    {

    }
    
    public DbSet<OrderModel> Orders {get; set;}
    public DbSet<SingleItemOrder> SingleItemOrders  {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var orderId = Guid.NewGuid();
        var parent = new OrderModel
        {
            Id = orderId,
            CustomerEmail = "rairacer@gmail",
            OrderDate = DateTime.Now,
            OrderStatus = "Pending Payment"
        };
        
        modelBuilder.Entity<OrderModel>().HasData(
            parent
        );
        
        modelBuilder.Entity<SingleItemOrder>().HasData(
            new SingleItemOrder
            {
                Id = Guid.NewGuid(),
                ParentId = orderId,
                ProductId = Guid.NewGuid(),
                Quantity = 10
                
            },
            
            new SingleItemOrder
            {
                Id = Guid.NewGuid(),
                ParentId = orderId,
                ProductId = Guid.NewGuid(),
                Quantity = 10
            }
        );
            
    }
    
}