namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IRefundRepository 
    {
        Task<OrderRefund?> GetOrderRefundByIdWithOrderAndPaymentAsync(int OrderRefundId);
        Task<ProductRefund?> GetProductRefundByIdWithOrderAndPaymentAsync(int OrderRefundId);
      
        }
}
