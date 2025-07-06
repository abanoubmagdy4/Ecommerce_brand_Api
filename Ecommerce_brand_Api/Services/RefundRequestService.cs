
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services
{
    public class OrderRefundService : BaseService<OrderRefund>, IOrderRefundService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IOrderRefundRepository _OrderRefundRepository;
        private readonly IMapper _mapper;

        public OrderRefundService(IUnitofwork unitOfWork, IMapper mapper) : base(unitOfWork.GetBaseRepository<OrderRefund>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _OrderRefundRepository = _unitOfWork.OrderRefund;
        }

        public async Task<ServiceResult> GetOrderRefundWithOrderAndPaymentAsync(int OrderRefundId)
        {
            var OrderRefund = await _OrderRefundRepository.GetByIdWithOrderAndPaymentAsync(OrderRefundId);

            if (OrderRefund == null)
                return ServiceResult.Fail("Refund request not found.");

            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Refund request found.",
                Data = OrderRefund
            };
        }

    }
}
