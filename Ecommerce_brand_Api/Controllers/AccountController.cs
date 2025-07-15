using Microsoft.AspNetCore.Identity;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IServiceUnitOfWork _serviceunitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // private readonly IMapper _mapper;

        public AccountController(IServiceUnitOfWork serviceunitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _serviceunitOfWork = serviceunitOfWork;
            _userService = _serviceunitOfWork.Users;
            _userManager = userManager;
            _roleManager = roleManager;
            //   _mapper = mapper;
        }


        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return BadRequest("Role name is required.");

            var roleExists = await _roleManager.RoleExistsAsync(dto.RoleName);
            if (roleExists)
                return BadRequest("Role already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole(dto.RoleName));
            if (!result.Succeeded)
                return StatusCode(500, $"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Ok($"Role '{dto.RoleName}' created successfully.");
        }


        [HttpPost("Login")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ServiceResult serviceResult = await _userService.LoginAsync(loginDto);

            if (!serviceResult.Success)
                return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, serviceResult.ErrorMessage!));

            return Ok(serviceResult.SuccessMessage);
        }

        [HttpPost("SendVerificationCodeAsync")]
        public async Task<ActionResult<ServiceResult>> SendVerificationCodeAsync([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(ServiceResult.Fail("Email is required."));

            var code = new Random().Next(100000, 999999).ToString();

            var saveResult = await _userService.SaveCodeAsync(email, code);
            if (!saveResult.Success)
                return StatusCode(500, saveResult); // Internal Error in DB

            var subject = "Login Code";
            var body = $"Your login code is: {code}";

            var emailResult = await _userService.SendEmailAsync(email, subject, body);
            if (!emailResult.Success)
                return StatusCode(500, emailResult); // Email failed

            return Ok(ServiceResult.Ok("A verification code has been sent to your email."));
        }



        [HttpPost("CustomerAccountLogin")]

        public async Task<ActionResult> CustomerAccountLogin([FromBody] CustomerLoginDto customerLoginDto)
        {


            ServiceResult serviceResult = await _userService.HandleCustomerLoginAsync(customerLoginDto);

            if (!serviceResult.Success) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, serviceResult.ErrorMessage!));

            return Ok(new { token = serviceResult.Data });

        }

        [HttpPost("AdminRegister")]
        public async Task<ActionResult> AdminRegister(RegisterDto registerDTO)
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
        public async Task<IActionResult> RequestResetCode([FromForm] PasswordResetRequestDTO dto)
        {
            var user = await _userService.FindByEmailAsync(dto.Email);
            if (user == null)
                return Ok("If this email exists, a code will be sent."); // ما تقولش للمستخدم إن الإيميل مش موجود

            var code = new Random().Next(100000, 999999).ToString();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _userService.SaveCodeAndTokenAsync(dto.Email, code, token);
            await _userService.SendEmailAsync(dto.Email, "Password  Reset Code", $"Your reset code is: {code}");

            return Ok("A reset code has been sent to your email.");
        }

        [HttpPost("reset-password-with-code")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ResetPasswordWithCode([FromForm] ResetPasswordWithCodeDto dto)
        {
            var isValid = await _userService.ValidateCodeAsync(dto.Email, dto.Code);
            if (!isValid)
                return BadRequest("Invalid or expired code.");

            var user = await _userService.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("User not found.");

            var token = await _userService.GetStoredResetTokenAsync(dto.Email);
            if (string.IsNullOrEmpty(token))
                return BadRequest("Reset token not found or expired.");

            var result = await _userService.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!result)
                return BadRequest("Reset failed. Please try again.");

            await _userService.DeleteCodeAsync(dto.Email); // نمسح السطر بعد نجاح العملية

            return Ok("Password has been reset successfully.");
        }


        [HttpGet("{customerId}/addresses")]
        public async Task<ActionResult> GetListOfAddressesByCustomerIdAsync(string customerId)
        {
            var result = await _userService.GetListOfAddressesByCustomerIdAsync(customerId);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); 
            }

            return Ok(result.Data); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            try
            {
                var customer = await _userService.GetOneCustomerAsync(id);
                return Ok(customer);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Customer with ID '{id}' not found." });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unexpected error occurred." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileById()
        {
            try
            {
                var userId = _userService.GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated or token is invalid." });
                }

                var customer = await _userService.GetProfileAsync(userId);

                if (customer == null)
                {
                    return NotFound(new { message = $"Customer not found." });
                }

                return Ok(customer);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Customer not found." });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = $"Application error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Unexpected error occurred: {ex.Message}" });
            }
        }



    }
}