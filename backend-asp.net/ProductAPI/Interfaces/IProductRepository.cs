using ProductAPI.Dtos.Game;
using ProductAPI.Model;

namespace ProductAPI.Interfaces;

public interface IProductRepository
{
    Task<List<ProductModel>> getAllAsync();

    // note : "?" => meaning could be null, in case not found
    Task<ProductModel?> getByIdAsync(Guid id);
    Task<ProductModel> createAsync(PostProductDto postProductDto);

    Task<ProductModel> updateAsync(Guid id, PutProductDto putProductDto);
    Task<ProductModel?> deleteAsync(Guid id);

    Task<bool> productExistAsync(Guid id);

    Task<ProductModel> reserveStockAsync(Guid id, int quantity);

    Task<ProductModel> consumeReservedStockAsync(Guid id, int quantity);
    
    Task<ProductModel> releaseReservedStockAsync(Guid id, int quantity);
    
}
