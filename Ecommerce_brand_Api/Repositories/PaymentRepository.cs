namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {

        }


        public async Task<Payment?> GetPaymentByTransactionIdAsync(long transactionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

    }
}
