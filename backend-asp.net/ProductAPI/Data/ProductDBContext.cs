using Microsoft.EntityFrameworkCore;
using ProductAPI.Model;

namespace ProductAPI.Data;

public class ProductDBContext : DbContext
{
    public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
    {

    }
    public DbSet<ProductModel> Products {get; set;}
    public DbSet<CommentModel> Comments  {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var productId = Guid.NewGuid();
        
        modelBuilder.Entity<ProductModel>().HasData(
            new ProductModel
            {
                Id = productId,
                Title = "Nike Shoes",
                Description = "Nike Shoes",
                Price = 50,
                Stock = 100,
                Category = "Shoes"
            }
        );
        
        modelBuilder.Entity<CommentModel>().HasData(
            new CommentModel
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                userId = Guid.Empty,
                Content = "Great product!",
                Rating = 5,
            }
        );
            
    }
}
