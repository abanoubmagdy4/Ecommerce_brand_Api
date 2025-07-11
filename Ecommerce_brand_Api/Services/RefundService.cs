
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Services
{
    public class RefundService :IRefundService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IRefundRepository _RefundRepository;
        private readonly IBaseRepository<ProductRefund> _productRefundBaseRepository;
        private readonly IBaseRepository<OrderRefund> _OrderRefundBaseRepository;
        private readonly IMapper _mapper;

        public RefundService(IUnitofwork unitOfWork, IMapper mapper, IBaseRepository<OrderRefund> orderRefundBaseRepository, IBaseRepository<ProductRefund> productRefundBaseRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _RefundRepository = _unitOfWork.Refund;
            _OrderRefundBaseRepository = orderRefundBaseRepository;
            _productRefundBaseRepository = productRefundBaseRepository;
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

        public async Task<ServiceResult> GetAllOrderRefund()
        {
            List<OrderRefund> orderRefunds = await _OrderRefundBaseRepository.GetAllAsync();
             if (orderRefunds == null) {
                return ServiceResult.Fail("NO Product Exist");
            }

            return ServiceResult.OkWithData(orderRefunds);

        }

        public async Task<ServiceResult> GetAllProductRefund()
        {

            List<ProductRefund> productRefunds = await _productRefundBaseRepository.GetAllAsync();
            if (productRefunds == null)
            {
                return ServiceResult.Fail("NO Product Exist");
            }

            return ServiceResult.OkWithData(productRefunds);

        }

    }
}
