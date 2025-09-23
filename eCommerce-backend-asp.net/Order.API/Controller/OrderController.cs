using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.API.Data;
using Order.API.Dtos;
using Order.API.GRPC;
using Order.API.Interfaces;
using Order.API.RabbitMqService;
using Order.API.Repository;
using Order.API.Services;
using Product;
using SharedLibrary.MassTransit.RabbitMQ;

namespace Order.API.Controller;

[Route("/order")]
[ApiController]
[Authorize]
public class OrderController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly OrderService _orderService;  // MassTransit Publisher
    
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger, OrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    // path : /orders/all
    [HttpGet, Route("all")]
    [Authorize(Policy = "OnlyAdmin")]
    public async Task<IActionResult> GetAll()
    {

        var orders = await _orderService.GetAllOrdersAsync();
        
        return Ok(orders);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByUserEmail()
    {
        
        var userEmail = HttpContext.Items["UserEmail"]?.ToString();
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        try
        {
            List<OrderModel> orders = await _orderService.GetOrdersByUserEmailAsync(userEmail);

            return Ok(orders);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(e.Message);
        }
        catch ( Exception e)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        //await _massTransitService.SendRequestForResponseAsync(1);

        try
        {
            OrderModel order = await _orderService.GetOrderByIdAsync(id);
        
            return Ok(order);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(e.Message);
        }
        catch ( Exception e)
        {
            return StatusCode(500, "Internal server error");
        }
        
    }
    
    
    
    [HttpPost]
    public async Task<IActionResult> PostSingleOrder([FromBody] PostOrderDto postOrderDto)
    {
        
        var userEmail = HttpContext.Items["UserEmail"]?.ToString();

        try
        {
            var createdOrder = await _orderService.CreateOrderAsync(userEmail, postOrderDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        // for other exceptions
        catch ( Exception e)
        {
            return StatusCode(500, "Internal server error");
        }
        
    }
}