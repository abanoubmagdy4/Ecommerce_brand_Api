namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IRefundRequestRepository:IBaseRepository<RefundRequest>    
    {
        Task<RefundRequest?> GetByIdWithOrderAndPaymentAsync(int refundRequestId);

    }
}
