using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {

        Task<IEnumerable<Order>> GetOrdersWithCustomerAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersOverTotalAsync(decimal amount);
        Task<Order> ChangeOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}