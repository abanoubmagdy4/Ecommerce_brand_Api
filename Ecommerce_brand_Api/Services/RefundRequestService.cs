
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services
{
    public class RefundRequestService : BaseService<RefundRequest>,IRefundRequestService   
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IRefundRequestRepository _refundRequestRepository;
        private readonly IMapper _mapper;

        public RefundRequestService(IUnitofwork unitOfWork, IMapper mapper) : base(unitOfWork.GetBaseRepository<RefundRequest>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _refundRequestRepository = _unitOfWork.RefundRequest;
        }

        public async Task<ServiceResult> GetRefundRequestWithOrderAndPaymentAsync(int refundRequestId)
        {
            var refundRequest = await _refundRequestRepository.GetByIdWithOrderAndPaymentAsync(refundRequestId);

            if (refundRequest == null)
                return ServiceResult.Fail("Refund request not found.");

            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Refund request found.",
                Data = refundRequest
            };
        }

    }
}
