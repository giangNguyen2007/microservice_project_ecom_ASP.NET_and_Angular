using Microsoft.EntityFrameworkCore;

namespace Payment.API.data;

public class PaymentRequestDbContext : DbContext
{
    public PaymentRequestDbContext(DbContextOptions<PaymentRequestDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<PaymentRequestModel> PaymentRequests { get; set; }
}