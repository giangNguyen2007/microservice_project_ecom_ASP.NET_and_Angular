using ProductAPI.Dtos.Game;
using ProductAPI.Model;

namespace ProductAPI.Dtos;

public static class ProductDtoMapper
{
    //convert from GameModel to GameDto
    public static GetProductDto ToGetGameDto(this ProductModel product)
    {
        return new GetProductDto
        {
            Id = product.Id,
            Description = product.Description,
            PhotoUrl = product.PhotoUrl,
            Category = product.Category,
            Title = product.Title,
            Price = product.Price,
            Stock = product.Stock,
            //Comments = product.Comments.Select(c => c.ToGetCommentDto()).ToList()
        };
    }
    
    public static ProductModel PostProductDto_to_Product(this PostProductDto postProductDto)
    {
        return new ProductModel
        {
            Title = postProductDto.Title,
            Description = postProductDto.Description,
            PhotoUrl = postProductDto.PhotoUrl,
            Category = postProductDto.Category,
            Price = postProductDto.Price,
            Stock = postProductDto.Stock,
        };
    }
}
