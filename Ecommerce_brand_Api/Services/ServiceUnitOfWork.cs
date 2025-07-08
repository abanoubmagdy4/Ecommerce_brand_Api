using Ecommerce_brand_Api.Models.Entities;
using Ecommerce_brand_Api.Repositories.Interfaces;

namespace Ecommerce_brand_Api.Services
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public ServiceUnitOfWork(IUnitofwork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        public IBaseService<T> GetBaseService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<IBaseService<T>>();
        }

        private ICartService _carts;
        public ICartService Carts => _carts ??= _serviceProvider.GetRequiredService<ICartService>();

        private ICategoryService _categories;
        public ICategoryService Categories => _categories ??= _serviceProvider.GetRequiredService<ICategoryService>();

        private IFeedbackService _feedbacks;
        public IFeedbackService Feedbacks => _feedbacks ??= _serviceProvider.GetRequiredService<IFeedbackService>();

        private IGovernrateShippingCostService _governratesShippingCosts;
        public IGovernrateShippingCostService GovernratesShippingCosts => _governratesShippingCosts ??= _serviceProvider.GetRequiredService<IGovernrateShippingCostService>();

        private IOrderService _orders;
        public IOrderService Orders => _orders ??= _serviceProvider.GetRequiredService<IOrderService>();

        private IProductService _products;
        public IProductService Products => _products ??= _serviceProvider.GetRequiredService<IProductService>();

        private IPaymentService _payment;
        public IPaymentService Payment => _payment ??= _serviceProvider.GetRequiredService<IPaymentService>();

        private ITokenService _tokens;
        public ITokenService Tokens => _tokens ??= _serviceProvider.GetRequiredService<ITokenService>();

        private IUserService _users;
        public IUserService Users => _users ??= _serviceProvider.GetRequiredService<IUserService>();


        private IRefundService _Refund;
        public IRefundService Refund => _Refund ??= _serviceProvider.GetRequiredService<IRefundService>();

        public Task<int> SaveChangesAsync()
        {
            return _unitOfWork.SaveChangesAsync();
        }
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

    }
}
