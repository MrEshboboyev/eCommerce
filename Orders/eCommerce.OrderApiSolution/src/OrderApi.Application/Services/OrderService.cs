using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services;

public class OrderService(IOrder orderInterface, 
    HttpClient httpClient, 
    ResiliencePipelineProvider<string> resiliencePipelineProvider) : IOrderService
{
    // GET PRODUCT
    public async Task<ProductDTO> GetProduct(int productId)
    {
        // Call Product API using HttpClient
        // Redirect this call to the API Gateway since product API is not response to outsiders.
        var getProduct = await httpClient.GetAsync($"https://localhost:5003/api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null!;

        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }
    
    // GET USER
    public async Task<AppUserDTO> GetUser(int userId)
    {
        // Call User API using HttpClient
        // Redirect this call to the API Gateway since user API is not response to outsiders.
        var getUser = await httpClient.GetAsync($"https://localhost:5003/api/authentication/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null!;

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }

    // GET ORDER DETAILS BY ID
    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        // Prepare Order
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order.Id <= 0)
            return null!;

        // Get retry pipeline
        var retryPipeline = resiliencePipelineProvider.GetPipeline("my-retry-pipeline");

        // Prepare Product
        var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

        // Prepare User
        var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

        // Populate Order Details
        return new OrderDetailsDTO
            (
            order.Id,
            productDTO.Id,
            appUserDTO.Id,
            appUserDTO.Name,
            appUserDTO.Email,
            appUserDTO.Address,
            appUserDTO.TelephoneNumber,
            productDTO.Name,
            order.PurchaseQuantity,
            productDTO.Price,
            productDTO.Quantity * order.PurchaseQuantity,
            order.OrderedDate
            );
    }

    // GET ORDERS BY CLIENT ID
    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
    {
        // Get all Client's orders
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);

        // Convert from Entity to DTO and return
        var (_, _orders) = OrderConversion.FromEntity(null, orders);
        return _orders!;
    }
}
