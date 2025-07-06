using static Ecommerce_brand_Api.Models.Dtos.Payment.WebhookRequestDto;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IPaymentService : IBaseService<Payment>
    {
        Task<Payment?> GetPaymentByTransactionIdAsync(long transactionId);
        Task HandleIncomingTransactionAsync(Transaction transaction);
        Task<ServiceResult> HandleOrderRequestRefundAsync(OrderRefundDto dto);
        Task<ServiceResult> HandleProductRequestRefundAsync(ProductRefundDto dto);
        Task<ServiceResult> HandleCheckoutAsync(OrderDTO orderDto);
        Task<ServiceResult> HandleApproveOrderRefund(ApproveOrderRefundDto dto);
        Task<ServiceResult> HandleApproveProductRefund(ApproveProductRefundDto dto);


            }
}
