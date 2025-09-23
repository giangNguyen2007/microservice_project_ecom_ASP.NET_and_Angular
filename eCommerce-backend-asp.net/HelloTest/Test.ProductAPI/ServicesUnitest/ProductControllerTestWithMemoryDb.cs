using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductAPI.Controllers;
using ProductAPI.Data;
using ProductAPI.Dtos.Game;
using ProductAPI.Model;
using ProductAPI.Repository;
using Xunit.Abstractions;

namespace HelloTest.Test.ProductAPI.ServicesUnitest;

public class InMemoryDbFixture
{
    
    public ProductDBContext DbContext;
    public ProductRepository ProductRepository;
    public ProductController ProductController;

    public InMemoryDbFixture()
    {
        
        
        // setup the database
        var options = new DbContextOptionsBuilder<ProductDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    
        DbContext = new ProductDBContext(options);
        
        // seed data 
        DbContext.Database.EnsureCreated();
        
        // Seed database
        DbContext.Products.AddRange(
            
            new ProductModel
            {
                Id = Guid.NewGuid(),
                Title = "Wireless Headphones",
                Description = "Over-ear noise-cancelling headphones with 30h battery life",
                PhotoUrl = "https://example.com/images/headphones.jpg",
                Category = "Electronics",
                Price = 120,
                Stock = 15
            },
            
            new ProductModel
            {
                Id = Guid.NewGuid(),
                Title = "Ergonomic Office Chair",
                Description = "Adjustable mesh office chair with lumbar support",
                PhotoUrl = "https://example.com/images/office_chair.jpg",
                Category = "Furniture",
                Price = 210,
                Stock = 5
            }
            
        );
        
        DbContext.SaveChanges();
        
        // create the controller & repo
        ProductRepository = new ProductRepository(DbContext);
        ProductController = new ProductController(ProductRepository, new LoggerFactory().CreateLogger<ProductController>());
    }
}


public class ProductControllerTestWithMemoryDb : IClassFixture<InMemoryDbFixture>
{
    private InMemoryDbFixture _fixture;
    private ITestOutputHelper _testOutputHelper;

    public ProductControllerTestWithMemoryDb(InMemoryDbFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task testGetAll()
    {

        var result = await _fixture.ProductController.GetAll();
        
        // assertion
        
        // Because the method return IActionResult
        // thus => have to extract the body like this
        OkObjectResult okResult =  Assert.IsType<OkObjectResult>(result);
        var productList = okResult.Value as IEnumerable<GetProductDto>; ;

        foreach (var prod in productList)
        {
            _testOutputHelper.WriteLine(prod.Title);
        }
        
    }
    
    [Fact]
    public async Task testCreateProduct()
    {
        var newProduct = new PostProductDto()
        {
            Title = "Wireless Headphones 2",
            Description = "Over-ear noise-cancelling headphones with 30h battery life",
            PhotoUrl = "https://example.com/images/headphones.jpg",
            Category = "Electronics",
            Price = 120,
            Stock = 15
        };
        var result = await _fixture.ProductController.PostSingleProduct(newProduct);
        
        
        OkObjectResult okResult =  Assert.IsType<OkObjectResult>(result);
       
        // check number of product in DB
        var allProducts = _fixture.DbContext.Products.ToList();

        Assert.Equal(4, allProducts.Count());
        
        // find the prodct and remove it from db
        // var myProd = await _fixture.DbContext.Products.FirstOrDefaultAsync(p => p.Title == "Wireless Headphones 2");
        // _fixture.DbContext.Products.Remove(myProd);
        // await _fixture.DbContext.SaveChangesAsync();
    }
}