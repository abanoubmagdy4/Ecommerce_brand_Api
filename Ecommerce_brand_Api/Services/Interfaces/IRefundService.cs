using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IRefundService
    {
        Task<ServiceResult> GetOrderRefundWithOrderAndPaymentAsync(int orderRefundId);
        Task<ServiceResult> GetProductRefundWithOrderAndPaymentAsync(int productRefundId);
        Task<ServiceResult> GetAllOrderRefund();
        Task<ServiceResult> GetAllProductRefund();
    }
}
