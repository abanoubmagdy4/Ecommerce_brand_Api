using static Ecommerce_brand_Api.Models.Dtos.Payment.WebhookRequestDto;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IPaymentService : IBaseService<Payment>
    {
        Task<Payment?> GetPaymentByTransactionIdAsync(long transactionId);
        Task HandleIncomingTransactionAsync(TransactionDto transaction);
        Task<ServiceResult> HandleRequestRefundAsync(RefundRequestDto dto);
        Task<ServiceResult> HandleCheckoutAsync(OrderDTO orderDto);
        Task<ServiceResult> HandleApproveRefund(ApproveRefundDto dto);
    }
}
