using Microsoft.EntityFrameworkCore;

namespace Auth.API.Model;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {

    }
    public DbSet<UserModel> Users {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        
        // Seed authors
        modelBuilder.Entity<UserModel>().HasData(
            new UserModel { Email = "admin@gmail.com", Password = "admin", Role = "admin"}
        );
    }

}