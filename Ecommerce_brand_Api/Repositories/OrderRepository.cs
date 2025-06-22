using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;
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
                .Include(o => o.Discount)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> ChangeOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new ApplicationException($"Order with ID {orderId} not found.");

            order.OrderStatus = newStatus;

            await _context.SaveChangesAsync();

            return order;
        }

    }

}