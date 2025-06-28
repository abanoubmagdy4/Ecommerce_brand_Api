
namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
      Task<ApplicationUser> FindByEmailAsync(string email); 
    }
}
