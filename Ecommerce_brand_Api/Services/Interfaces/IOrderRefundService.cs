using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IOrderRefundService : IBaseService<OrderRefund>
    {
        Task<ServiceResult> GetOrderRefundWithOrderAndPaymentAsync(int OrderRefundId);


    }
}
