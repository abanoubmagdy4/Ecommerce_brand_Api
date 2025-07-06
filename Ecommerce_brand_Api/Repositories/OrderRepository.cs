using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse;
using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {

        public OrderRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders.Where(o => o.OrderStatus == status).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersOverTotalAsync(decimal amount)
        {
            return await _context.Orders.Where(o => o.TotalOrderPrice > amount).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersWithCustomerAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
        public async Task<OrderItemWithStatusAndAmountAndQuantityDto?> GetOrderItemWithOrderStatusWithPaymentStatuAndAmmountAsync(int orderItemId)
        {
            return await _context.OrderItems
                .Where(o => o.OrderItemId == orderItemId)
                .Select(o => new OrderItemWithStatusAndAmountAndQuantityDto
                {
                    OrderItemId = o.OrderItemId,
                    OrderStatus = o.Order.OrderStatus,
                    PaymentStatus = o.Order.Payment.Status,
                    ShippingStatus = o.Order.ShippingStatus,
                    TotalPrice = o.TotalPrice,
                    Quantity = o.Quantity
                    
                })
                .FirstOrDefaultAsync();
        }


        public async Task<Order?> GetOrderByPaymobOrderIdAsync(int paymobOrderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.PaymobOrderId == paymobOrderId && !o.IsDeleted);
        }
        public async Task<Order?> GetOrderWithPaymentAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

    }

}