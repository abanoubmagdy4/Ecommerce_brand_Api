namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
       // private readonly IMapper _mapper;

        public AccountController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
         //   _mapper = mapper;
        }

        [HttpPost("Login")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ServiceResult serviceResult = await _userService.LoginAsync(loginDto);

            if (!serviceResult.Success)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, serviceResult.ErrorMessage!));

            return Ok(serviceResult.SuccessMessage);
        }


        [HttpPost("Register")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDTO)
        {
            if (ModelState.IsValid)
            {
                ServiceResult serviceResult = await _userService.RegisterAsync(registerDTO, "Customer");
                if (!serviceResult.Success) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, serviceResult.ErrorMessage!));
                return Ok("User Register Success");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("AdminRegister")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> AdminRegister([FromBody] RegisterDto registerDTO)
        {
            if (ModelState.IsValid)
            {
                ServiceResult serviceResult = await _userService.RegisterAsync(registerDTO, "Admin");
                if (!serviceResult.Success) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, serviceResult.ErrorMessage!));
                return Ok("User Register Success");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("request-reset-code")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RequestResetCode([FromBody] PasswordResetRequestDTO dto)
        {
            var user = await _userService.FindByEmailAsync(dto.Email);
            if (user == null)
                return Ok("If this email exists, a code will be sent.");

            var code = new Random().Next(100000, 999999).ToString();

            await _userService.SaveCodeAsync(dto.Email, code);
            await _userService.SendEmailAsync(dto.Email, "Password Reset Code", $"Your reset code is: {code}");

            return Ok("A reset code has been sent to your email.");
        }

        [HttpPost("reset-password-with-code")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordWithCodeDto dto)
        {
            var isValid = await _userService.ValidateCodeAsync(dto.Email, dto.Code);
            if (!isValid)
                return BadRequest("Invalid or expired code.");

            var user = await _userService.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("User not found.");

            var token = await _userService.GeneratePasswordResetTokenAsync(user);
            var result = await _userService.ResetPasswordAsync(user, token, dto.NewPassword);



            await _userService.DeleteCodeAsync(dto.Email);

            return Ok("Password has been reset successfully.");
        }
    }
}