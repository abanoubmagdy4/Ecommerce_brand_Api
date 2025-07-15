
namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<List<Address>> GetListOfAddressesByCustomerIdAsync(string customerId);
        Task<CustomerDto?> GetOneCustomerAsync(string customerId);
            }
}
