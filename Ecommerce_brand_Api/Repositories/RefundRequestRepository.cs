namespace Ecommerce_brand_Api.Repositories
{
    public class OrderRefundRepository : BaseRepository<OrderRefund>, IOrderRefundRepository
    {
        
            public OrderRefundRepository(AppDbContext context) : base(context)
            {
            }



        public async Task<OrderRefund?> GetByIdWithOrderAndPaymentAsync(int OrderRefundId)
        {
            return await _context.OrderRefund
                                 .Include(r => r.Payment)
                                 .ThenInclude(o => o.Order)
                                 .FirstOrDefaultAsync(r => r.Id == OrderRefundId);
        }

    }
}
