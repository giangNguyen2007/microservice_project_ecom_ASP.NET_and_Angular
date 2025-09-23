using Grpc.Core;
using ProductAPI.Data;
using ProductAPI.Dtos.Game;
using ProductAPI.Interfaces;
using ProductAPI.Model;
using ProductAPI.Repository;
using ProductAPI.Services;
using YourApp.Grpc;

namespace ProductAPI.GRPC.Service;

public class GrpcProductServer : ProductService.ProductServiceBase
{
    private ProductStockService _productStockService;

    public GrpcProductServer(ProductStockService productStockService)
    {
        _productStockService = productStockService;
    }

    public override async Task<ReservationReply> ReserveProductStock(ReservationRequest request, ServerCallContext context)
    {

        Console.WriteLine($"Received Message: Product Reservation request = {request.OrderItemList}");
            
        try
        {
            
            await _productStockService.UpdateProductStockAsync(request.OrderItemList, UpdateStockAction.ReserveStock);
            
            return new ReservationReply
            {
                Success = true,
                ErrorMessage = ""
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            
            return new ReservationReply
            {
                Success = false,
                ErrorMessage = e.Message
            };
        
        }

    }
    
    public override async Task<StockUpdateReply> UpdateProductStock(StockUpdateRequest request, ServerCallContext context)
    {

        // create list of products to be updated
        Console.WriteLine($"Received Message: Product Stock Update request = {request.OrderItemList}");
        
        try
        {
            await _productStockService.UpdateProductStockAsync(request.OrderItemList, UpdateStockAction.ConSumeReservedStock);

            return new StockUpdateReply
            {
                UpdateSuccess = true,
                ErrorMessage = ""
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return new StockUpdateReply
            {
                UpdateSuccess = false,
                ErrorMessage = e.Message
            };
            
        }
    }
    
    public override async Task<StockReleaseReply> ReleaseProductStock(StockRealeaseRequest request, ServerCallContext context)
    {

        // create list of products to be updated
        Console.WriteLine($"Received Message: Product Stock RELEASE request = {request.OrderItemList}");
        
        try
        {
            await _productStockService.UpdateProductStockAsync(request.OrderItemList, UpdateStockAction.ReleaseReservedStock);

            return new StockReleaseReply()
            {
                Success = true,
                ErrorMessage = ""
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return new StockReleaseReply
            {
                Success = false,
                ErrorMessage = e.Message
            };
            
        }
        
    }

}