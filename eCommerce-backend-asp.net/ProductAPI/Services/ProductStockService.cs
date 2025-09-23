using Google.Protobuf.Collections;
using ProductAPI.Data;
using ProductAPI.Interfaces;
using YourApp.Grpc;

namespace ProductAPI.Services;

public enum UpdateStockAction
{
    ReserveStock,
    ConSumeReservedStock,
    ReleaseReservedStock
}


// Centralize stock management logic here
// This class can include methods to check stock, reserve stock, release stock, etc.
public class ProductStockService
{
    
    
    private IProductRepository _productRepository;
    private ProductDBContext _productDbContext;
    
    public ProductStockService(IProductRepository productRepository, ProductDBContext productDbContext)
    {
        _productRepository = productRepository;
        _productDbContext = productDbContext;
    }
    
    // 
    public async Task UpdateProductStockAsync( RepeatedField<SingleOrderItem> orderItems , UpdateStockAction action)
    {
        // wrap in transaction to ensure atomicity
        using (var transaction = await _productDbContext.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var item in orderItems)
                {
                    if (!Guid.TryParse(item.ProductId, out _))
                        throw new ArgumentException($"Invalid GUID format for { item.ProductId}");

                    switch (action)
                    {
                        case UpdateStockAction.ReserveStock:
                            
                            // all checks are handled in the repository level
                            await _productRepository.reserveStockAsync(Guid.Parse(item.ProductId), item.Quantity);
                        
                            break;

                        case UpdateStockAction.ConSumeReservedStock:
                            
                            // all checks are handled in the repository level
                            await _productRepository.consumeReservedStockAsync(Guid.Parse(item.ProductId), item.Quantity);
                            
                            break;

                        case UpdateStockAction.ReleaseReservedStock:
                            
                            // all checks are handled in the repository level
                            await _productRepository.releaseReservedStockAsync(Guid.Parse(item.ProductId), item.Quantity);
                            
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(action), action, null);
                
                    }
            
            
                }
                
                await transaction.CommitAsync();
            }
            catch
            {
                // this block allows to rollback transaction if any exception occurs
                await transaction.RollbackAsync();
                
                // before rethrow the exception to be handled by the caller
                throw; 
            }
        }

        
    }
    
    
}