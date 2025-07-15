
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
            ApplicationUser applicationUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return applicationUser;
        }

        public async Task<List<Address>> GetListOfAddressesByCustomerIdAsync(string customerId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == customerId)
                .ToListAsync();
        }
        public async Task<CustomerDto?> GetOneCustomerAsync(string customerId)
        {
            return await _context.Users
                .Where(c => c.Id == customerId)
                .Select(c => new CustomerDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    DateOfBirth = c.DateOfBirth
                })
                .FirstOrDefaultAsync();
        }
        public async Task<ProfileDto?> GetProfileAsync(string customerId)
        {
            return await _context.Users
                .Where(c => c.Id == customerId)
                .Select(c => new ProfileDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    DateOfBirth = c.DateOfBirth
                })
                .FirstOrDefaultAsync();
        }


    }

}
