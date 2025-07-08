namespace Ecommerce_brand_Api.Repositories
{
    public class RefundRepository : IRefundRepository
    {
        private readonly AppDbContext _context; 
            public RefundRepository(AppDbContext context) 
            {

            _context = context;


            }

        public async Task<OrderRefund?> GetOrderRefundByIdWithOrderAndPaymentAsync(int OrderRefundId)
        {
            return await _context.OrderRefund
                                 .Include(r => r.Payment)
                                 .ThenInclude(o => o.Order)
                                 .FirstOrDefaultAsync(r => r.Id == OrderRefundId);
        }
        public async Task<ProductRefund?> GetProductRefundByIdWithOrderAndPaymentAsync(int OrderRefundId)
        {
            return await _context.ProductRefund
                                 .Include(r => r.OrderItem)
                                 .ThenInclude(o => o.Order)
                                 .ThenInclude(o => o.Payment)
                                 .FirstOrDefaultAsync(r => r.Id == OrderRefundId);
        }
    }
}
