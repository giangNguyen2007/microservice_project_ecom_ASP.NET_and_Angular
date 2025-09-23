using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Dtos;
using ProductAPI.Dtos.Game;
using ProductAPI.Interfaces;
using ProductAPI.Model;

namespace ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ProductDBContext _context;

    public ProductRepository(ProductDBContext context)
    {
        _context = context;
    }


    public async Task<List<ProductModel>> getAllAsync()
    {
        return await _context.Products.Include(c => c.Comments).ToListAsync();
   
    }

    public async Task<ProductModel?> getByIdAsync(Guid id)
    {
        var ProductModel = await _context.Products
            .Include(c => c.Comments)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return ProductModel;
    }

    public async Task<ProductModel> createAsync(PostProductDto postProductDto)
    {
        ProductModel newProduct = postProductDto.PostProductDto_to_Product();
        await _context.Products.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        return newProduct;
    }

    public async Task<ProductModel> updateAsync(Guid id, PutProductDto putProductDto)
    {
        
        // wrap in transaction to ensure atomicity
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var myProduct = await _context.Products.FirstOrDefaultAsync(g => g.Id == id);
            if (myProduct == null)
                throw new KeyNotFoundException("Product not found");
        

            if (putProductDto.Title != null) { myProduct.Title = putProductDto.Title; }
            if (putProductDto.Description != null) { myProduct.Description = putProductDto.Description; }
            if (putProductDto.PhotoUrl != null) { myProduct.PhotoUrl = putProductDto.PhotoUrl; }

            if (putProductDto.Stock != null)
            {
                if (putProductDto.Stock > myProduct.Stock)
                    throw new InvalidOperationException("Not enough stock");
                myProduct.Stock -= putProductDto.Stock.Value;
            }

 
            await _context.SaveChangesAsync();

            return myProduct;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
        
    }
    public async Task<ProductModel?> deleteAsync(Guid id)
    {
        var myProduct = await _context.Products.FirstOrDefaultAsync(g => g.Id == id);
        if (myProduct == null)
        {
            return null;
        }

        // Note: remove is not an async function, leave it like this
        _context.Products.Remove(myProduct);
        await _context.SaveChangesAsync();

        return myProduct;
    }

    public Task<bool> productExistAsync(Guid id)
    {
        return _context.Products.AnyAsync(g => g.Id == id);
    }
    
    // creation of reserveStock allows to reserve stock when an order is created
    // and then later on, when payment is confirmed, the stock is actually decremented
    // this avoid the problem of selling more products than we have in stock
    public async Task<ProductModel> reserveStockAsync(Guid id, int quantity)
    {
        // transaction is performed at a higher level, in the gRPC service
            
        var myProduct = await _context.Products.FirstOrDefaultAsync(g => g.Id == id);
        if (myProduct == null)
            throw new KeyNotFoundException("Product not found");
    

        if (quantity > myProduct.Stock)
            throw new InvalidOperationException("Not enough stock");
    
        myProduct.Stock -= quantity;
        myProduct.ReservedStock += quantity;
        
        await _context.SaveChangesAsync();
        
        return myProduct;
            
        
    }
    
    // consumeReservedStock is called when payment is confirmed
 
    public async Task<ProductModel> consumeReservedStockAsync(Guid id, int quantity)
    {
            
        // transaction is performed at the caller level
        var myProduct = await _context.Products.FirstOrDefaultAsync(g => g.Id == id);
        if (myProduct == null)
            throw new KeyNotFoundException("Product not found");
        
        if (quantity > myProduct.ReservedStock)
            throw new InvalidOperationException("Not enough reserved stock to consume");
        
        myProduct.ReservedStock -= quantity;
        
        await _context.SaveChangesAsync();
        
        return myProduct;
        
    }
    
    // releaseStock is used when an order is cancelled or payment fails
    // it puts back the reserved stock to the available stock
    public async Task<ProductModel> releaseReservedStockAsync(Guid id, int quantity)
    {
            
        // transaction is performed at the caller level
        var myProduct = await _context.Products.FirstOrDefaultAsync(g => g.Id == id);
        if (myProduct == null)
            throw new KeyNotFoundException("Product not found");
    

        if (quantity > myProduct.ReservedStock)
            throw new InvalidOperationException("Not enough reserved stock to release");
    
        myProduct.Stock += quantity;
        myProduct.ReservedStock -= quantity;
        
        await _context.SaveChangesAsync();
        
        return myProduct;
        
    }
}
