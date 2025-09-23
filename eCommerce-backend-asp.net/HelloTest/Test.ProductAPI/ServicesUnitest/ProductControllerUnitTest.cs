using Microsoft.AspNetCore.Mvc;
using Moq;
using Order.API.Controller;
using ProductAPI.Controllers;
using ProductAPI.Dtos.Game;
using ProductAPI.Interfaces;
using ProductAPI.Model;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HelloTest.Test.ProductAPI.ServicesUnitest;

// create fixture => so that those instances are created once per test class
// those instances are shared across all tests
public class RepoMockingFixture
{
    public Mock<IProductRepository> MockRepo { get; }
    public ProductController Controller { get; }

    public RepoMockingFixture()
    {
        MockRepo = new Mock<IProductRepository>();
        
        // inject the
        Controller = new ProductController(MockRepo.Object);
    }
}

// mocking productRepository
public class ProductControllerUnitTest : IClassFixture<RepoMockingFixture>
{
    private RepoMockingFixture _fixture;
    private ITestOutputHelper _testOutputHelper;

    public ProductControllerUnitTest(RepoMockingFixture fixture, ITestOutputHelper testOutputHelper)
    {
        // Fixture class is instantiated once by test runner then injected into the Test class
        // Test class instance is created multiple times (one per test),
        // but get injected with the same Fixture instance
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;

        // code in this method is run for each test methods
        // if run code below, the instance is created for each test => not good
        // _mockRepo = new Mock<IProductRepository>(); 
    }
    
    [Fact]
    public async Task testGetAll()
    {
   
        // setup return value
        _fixture.MockRepo.Setup(r => r.getAllAsync())
            .ReturnsAsync(new List<ProductModel>
            {
                new ProductModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Wireless Headphones",
                    Description = "Over-ear noise-cancelling headphones with 30h battery life",
                    PhotoUrl = "https://example.com/images/headphones.jpg",
                    Category = "Electronics",
                    Price = 120,
                    Stock = 15
                }
            });
        
        // call method
        IActionResult result = await _fixture.Controller.GetAll();
        
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
}