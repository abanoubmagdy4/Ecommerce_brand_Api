namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IPaymentRepository: IBaseRepository<Payment>
    {
        Task<Payment?> GetPaymentByTransactionIdAsync(long transactionId);

    }
}
