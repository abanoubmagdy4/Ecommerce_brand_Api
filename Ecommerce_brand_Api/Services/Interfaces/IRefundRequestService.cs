using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IRefundRequestService : IBaseService<RefundRequest>
    {
        Task<ServiceResult> GetRefundRequestWithOrderAndPaymentAsync(int refundRequestId);


    }
}
