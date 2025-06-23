namespace Ecommerce_brand_Api.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }

    }
        
}
