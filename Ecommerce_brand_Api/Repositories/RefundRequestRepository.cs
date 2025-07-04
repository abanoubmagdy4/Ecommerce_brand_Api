namespace Ecommerce_brand_Api.Repositories
{
    public class RefundRequestRepository : BaseRepository<RefundRequest>, IRefundRequestRepository
    {
        
            public RefundRequestRepository(AppDbContext context) : base(context)
            {
            }



        public async Task<RefundRequest?> GetByIdWithOrderAndPaymentAsync(int refundRequestId)
        {
            return await _context.RefundRequest
                                 .Include(r => r.Payment)
                                 .ThenInclude(o => o.Order)
                                 .FirstOrDefaultAsync(r => r.Id == refundRequestId);
        }

    }
}
