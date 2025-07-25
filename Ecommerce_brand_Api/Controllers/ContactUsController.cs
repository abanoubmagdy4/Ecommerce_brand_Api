namespace Ecommerce_brand_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IServiceUnitOfWork _serviceunitOfWork;


        // private readonly IMapper _mapper;

        public ContactUsController(IServiceUnitOfWork serviceunitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _serviceunitOfWork = serviceunitOfWork;
            _userService = _serviceunitOfWork.Users;
 
        }




    }
}
