using OrderApi.Domain.Entities;
using System.Data;

namespace OrderApi.Application.DTOs.Conversions;

public static class OrderConversion
{
    public static Order ToEntity(OrderDTO order) => new()
    {
        Id = order.Id,
        ClientId = order.ClientId,
        OrderedDate = order.OrderedDate,
        ProductId = order.ProductId,
        PurchaseQuantity = order.PurchaseQuantity
    };

    public static (OrderDTO, IEnumerable<OrderDTO>) FromEntity(
        Order order, IEnumerable<Order> orders)
    {
        // return single
        if (order is not null || orders is null)
        {
            var singleOrder = new OrderDTO
                (
                order!.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderedDate
                );

            return (singleOrder, null);
        }

        // return list
        if (order is null || orders is not null)
        {
            var _orders = orders.Select(o =>
                new OrderDTO(o!.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderedDate)
                ).ToList();

            return (null, _orders);
        }

        return (null, null);
    }
}
