using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.Enums;
using Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse;
using Ecommerce_brand_Api.Models.Entities;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

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
                    CreatedAt = o.Order.CreatedAt,
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
        
        public async Task<Order?> GetOrderByTransactionIdAsync(long transactionId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.PaymobOrderId == transactionId && !o.IsDeleted);
        }
        public async Task<PagedResult<OrderSummaryDto>> GetOrderSummariesAsync(OrderFilterDto filter)
        {
            var result = new PagedResult<OrderSummaryDto>();
            var connection = _context.Database.GetDbConnection();

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetOrderSummaries";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PageNumber", filter.PageNumber));
                command.Parameters.Add(new SqlParameter("@PageSize", filter.PageSize));
                command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)filter.SearchTerm ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@StatusTerm", (object?)filter.StatusTerm ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FromDate", (object?)filter.FromDate ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ToDate", (object?)filter.ToDate ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@SortDirection", (object?)filter.SortDirection ?? "DESC"));

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                // اقرأ TotalCount
                if (await reader.ReadAsync())
                    result.TotalCount = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);

                // انتقل للنتائج نفسها
                await reader.NextResultAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new OrderSummaryDto
                    {
                        OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("OrderId")),
                        OrderNumber = reader.IsDBNull(reader.GetOrdinal("OrderNumber")) ? null : reader.GetString(reader.GetOrdinal("OrderNumber")),
                        CustomerFullName = reader.IsDBNull(reader.GetOrdinal("CustomerFullName")) ? null : reader.GetString(reader.GetOrdinal("CustomerFullName")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        DeliveredAt = reader.IsDBNull(reader.GetOrdinal("DeliveredAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DeliveredAt")),
                        OrderAddressInfo = reader.IsDBNull(reader.GetOrdinal("OrderAddressInfo")) ? null : reader.GetString(reader.GetOrdinal("OrderAddressInfo")),
                        TransactionId = reader.IsDBNull(reader.GetOrdinal("TransactionId"))? (long?)null: reader.GetInt64(reader.GetOrdinal("TransactionId")),

                        PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString(reader.GetOrdinal("PaymentStatus")),
                        PaidAmount = reader.IsDBNull(reader.GetOrdinal("PaidAmount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                        PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? null : reader.GetString(reader.GetOrdinal("PaymentMethod")),

                        ShippingCost = reader.IsDBNull(reader.GetOrdinal("ShippingCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ShippingCost")),
                        TotalOrderPrice = reader.IsDBNull(reader.GetOrdinal("TotalOrderPrice")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalOrderPrice")),
                        DiscountValue = reader.IsDBNull(reader.GetOrdinal("DiscountValue")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("DiscountValue")),

                        OrderStatus = reader.IsDBNull(reader.GetOrdinal("OrderStatus")) ? null : reader.GetString(reader.GetOrdinal("OrderStatus")),
                        ShippingStatus = reader.IsDBNull(reader.GetOrdinal("ShippingStatus")) ? null : reader.GetString(reader.GetOrdinal("ShippingStatus")),

                        PaymobOrderId = reader.IsDBNull(reader.GetOrdinal("PaymobOrderId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PaymobOrderId")),
                        PaymobOrderReference = reader.IsDBNull(reader.GetOrdinal("PaymobOrderReference")) ? null : reader.GetString(reader.GetOrdinal("PaymobOrderReference")),

                        IsRefunded = reader.IsDBNull(reader.GetOrdinal("IsRefunded")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsRefunded")),
                        IsCanceled = reader.IsDBNull(reader.GetOrdinal("IsCanceled")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsCanceled")),

                        MatchScore = reader.IsDBNull(reader.GetOrdinal("MatchScore")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("MatchScore")),

                        Items = JsonConvert.DeserializeObject<List<OrderItemDTO>>(
                            reader.IsDBNull(reader.GetOrdinal("ItemsJson")) ? "[]" : reader.GetString(reader.GetOrdinal("ItemsJson"))
                        ) ?? new()
                    };

                    result.Items.Add(dto);
                }
            }

            return result;
        }

        public async Task<List<PreviousOrderDto>> GetListOfPreviousOrderByUserIdAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId && !o.IsDeleted)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImagesPaths)
                        .OrderByDescending(p=>p.CreatedAt)
                        .AsNoTracking() 
                .Select(o => new PreviousOrderDto
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    CreatedAt = o.CreatedAt,
                    DeliveredAt = o.DeliveredAt,
                    ShippingCost = o.ShippingCost,
                    DiscountValue = o.DiscountValue,
                    TotalOrderPrice = o.TotalOrderPrice,
                    paymentMethod = o.paymentMethod.ToString(),
                    OrderStatus = o.OrderStatus.ToString(),
                    ShippingStatus = o.ShippingStatus.ToString(),
                    OrderAddressInfo = o.OrderAddressInfo,
                    RefundStatuses = o.Payment.OrderRefunds
                         .Select(r => r.Status.ToString())
                         .ToList(),

                    OrderItems = o.OrderItems.Select(oi => new PreviousOrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Size = oi.productSize.Size,
                        width = oi.productSize.Width,
                        Height = oi.productSize.Height,
                        RefundStatus = oi.productRefund.Status.ToString(),
                        TotalPrice = oi.TotalPrice,
                        ImagePath = oi.Product.ProductImagesPaths
                                        .Select(im => im.ImagePath)
                                        .FirstOrDefault() ?? string.Empty
                    }).ToList()
                })
                .ToListAsync();

       
            return orders;
        }


    }

}