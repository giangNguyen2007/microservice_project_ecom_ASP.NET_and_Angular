using Grpc.Core;
using Order.API.Data;
using Order.API.Dtos;
using Product;

namespace Order.API.GRPC;

public class GrpcClientService
{
    private readonly ProductService.ProductServiceClient _productServiceClient;

    public GrpcClientService(ProductService.ProductServiceClient productServiceClient)
    {
        _productServiceClient = productServiceClient;
    }
    
    
    
    
    public async Task<ReservationReply?> SendReservationRequest(PostOrderDto postOrderDto)
    {
        
        var reservationRequest = new ReservationRequest();
        foreach (var item in postOrderDto.Items)
        {
            reservationRequest.OrderItemList.Add(new SingleOrderItem // from Product.proto, avoid confusion
            {
                ProductId = item.ProductId.ToString(),
                Quantity = item.Quantity
            });
        }

        try
        {
            ReservationReply reservationReply = await _productServiceClient.ReserveProductStockAsync(reservationRequest);
            return reservationReply;
        }
        catch (RpcException ex)
        {
            throw; // for clarity
        }
        
    }
    
    public async Task<StockUpdateReply?> SendStockUpdateRequest(OrderModel order)
    {

        var request = new StockUpdateRequest();
        foreach (var item in order.OrderItemList)
        {
            request.OrderItemList.Add(new SingleOrderItem // from Product.proto, avoid confusion
            {
                ProductId = item.ProductId.ToString(),
                Quantity = item.Quantity
            });
        }

        try
        {
            var reply = await _productServiceClient.UpdateProductStockAsync(request);
            return reply;
        }
        catch (RpcException ex)
        {
            throw; // for clarity
        }
        
    }
    
    public async Task<StockReleaseReply?> SendStockReleaseRequest(OrderModel order)
    {

        var request = new StockRealeaseRequest();
        foreach (var item in order.OrderItemList)
        {
            request.OrderItemList.Add(new SingleOrderItem // from Product.proto, avoid confusion
            {
                ProductId = item.ProductId.ToString(),
                Quantity = item.Quantity
            });
        }

        try
        {
            var reply = await _productServiceClient.ReleaseProductStockAsync(request);
            return reply;
        }       
        catch (RpcException ex)
        {
            throw; // for clarity
        }
        
    }
}