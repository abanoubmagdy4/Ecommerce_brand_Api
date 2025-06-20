using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
