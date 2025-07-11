using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse;
using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {

        Task<IEnumerable<Order>> GetOrdersWithCustomerAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersOverTotalAsync(decimal amount);
        Task<Order?> GetOrderByPaymobOrderIdAsync(int paymobOrderId);
        Task<Order?> GetOrderWithPaymentAsync(int orderId);
        Task<OrderItemWithStatusAndAmountAndQuantityDto?> GetOrderItemWithOrderStatusWithPaymentStatuAndAmmountAsync(int orderItemId);
        Task<Order?> GetOrderByTransactionIdAsync(long transactionId);
        Task<PagedResult<OrderSummaryDto>> GetOrderSummariesAsync(OrderFilterDto filter);

    }
}