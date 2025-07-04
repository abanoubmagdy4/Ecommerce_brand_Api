
using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
             ApplicationUser applicationUser =await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
           
            return  applicationUser;
        }

    }
        
}
