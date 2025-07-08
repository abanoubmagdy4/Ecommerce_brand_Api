
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services
{
    public class RefundService :IRefundService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IRefundRepository _RefundRepository;

        private readonly IMapper _mapper;

        public RefundService(IUnitofwork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _RefundRepository = _unitOfWork.Refund;
        }

        public async Task<ServiceResult> GetOrderRefundWithOrderAndPaymentAsync(int orderRefundId)
        {
            var orderRefund = await _RefundRepository.GetOrderRefundByIdWithOrderAndPaymentAsync(orderRefundId);

            if (orderRefund == null)
                return ServiceResult.Fail("Refund request not found.");

            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Refund request found.",
                Data = orderRefund
            };
        }
        public async Task<ServiceResult> GetProductRefundWithOrderAndPaymentAsync(int productRefundId)
        {
            var productRefund = await _RefundRepository.GetProductRefundByIdWithOrderAndPaymentAsync(productRefundId);

            if (productRefund == null)
                return ServiceResult.Fail("Refund request not found.");

            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Refund request found.",
                Data = productRefund
            };
        }

    }
}
