namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IOrderRefundRepository : IBaseRepository<OrderRefund>    
    {
        Task<OrderRefund?> GetByIdWithOrderAndPaymentAsync(int OrderRefundId);

    }
}
