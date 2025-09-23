using ProductAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Dtos.Game;
using ProductAPI.Interfaces;
using ProductAPI.Model;

namespace ProductAPI.Controllers;


[Route("/product")]
[ApiController]
[Authorize]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepo;
    
    private readonly ILogger<ProductController> _logger;
  

    public ProductController( IProductRepository productRepo, ILogger<ProductController> logger)
    {
        _productRepo = productRepo;
        _logger = logger;
    }

    //  === GET ====

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<ProductModel> products = await _productRepo.getAllAsync();

        // Select :  convert list of GameModel to IEnumerable of GetGameDto
        IEnumerable<GetProductDto> productsDto = products.Select(s => s.ToGetGameDto());
        
        return Ok(productsDto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        //await _massTransitService.SendRequestForResponseAsync(1);
        ProductModel? product = await _productRepo.getByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = "OnlyAdmin")]
    public async Task<IActionResult> PostSingleProduct([FromBody] PostProductDto postProductDto)
    {
        
        var userEmail = HttpContext.Items["UserEmail"]?.ToString();
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userRole))
        {
            _logger.LogError("Missing user email or role in HttpContext.Items.");
            return Unauthorized("Missing user email or role.");
        }
        _logger.LogInformation("User {UserEmail} is creating a new product.", userEmail);
        
        ProductModel newProduct = await _productRepo.createAsync(postProductDto);

        return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSingleProduct([FromRoute] Guid id, [FromBody] PutProductDto putProductDto)
    {
        try
        {
            var product = await _productRepo.updateAsync(id, putProductDto);
            return Ok(product.ToGetGameDto());
        }
        catch (KeyNotFoundException e)
        {
            return NotFound("Product not found");
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)  // catch all other exceptions , including db error
        {
            return BadRequest(e.Message);
        }
        
     
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSingleProduct([FromRoute] Guid id)
    {

        ProductModel? product = await _productRepo.deleteAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
