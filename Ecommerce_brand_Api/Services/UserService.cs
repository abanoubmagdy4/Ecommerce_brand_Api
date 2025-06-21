using Ecommerce_brand_Api.Helpers;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net;
using System.Net.Mail;


namespace Ecommerce_brand_Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IConfiguration config;
        private readonly EmailSettings _emailSettings;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           ITokenService tokenService,
                           AppDbContext context,
                           IConfiguration config,
                           IOptions<EmailSettings> options,
                           RoleManager<IdentityRole> roleManager
                          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            this.config = config;
            _emailSettings = options.Value;
            _roleManager = roleManager;
        }


        public async Task<ServiceResult> LoginAsync([FromForm] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return ServiceResult.Fail("Invalid Email or Password");

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                return ServiceResult.Fail("Invalid Email or Password");

            var  userRoles = await _userManager.GetRolesAsync(user);
            var tokenExpiration = loginDto.IsRemember ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1);
                     
            // Generate JWT Token 
            var token = _tokenService.CreateToken(user, userRoles, tokenExpiration);

            return ServiceResult.Ok(token);

            // Remember Me !!!!!!!!!!
        }


        public async Task<ServiceResult> RegisterAsync([FromForm] RegisterDto registerDTO, string userRole)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            //List<GovernorateShippingCost> governorateShippingCosts = new List<GovernorateShippingCost>();
            //foreach (var address in registerDTO.Addresses)
            //{
            //    var governorateShippingCost = _mapper.Map<GovernorateShippingCost>(address.GovernrateShippingCostDto);
            //    if (governorateShippingCost != null)
            //    {
            //        governorateShippingCosts.Add(governorateShippingCost);
            //    }
            //}
            if (existingUser != null)
                return ServiceResult.Fail("Email ALready Exists !");

            List<Address> addresses = new List<Address>();

            foreach (var address in registerDTO.Addresses)
            {
                Address add = new Address()
                {
                    Apartment = address.Apartment,
                    Building = address.Building,
                    City = address.City,
                    Country = address.Country,
                    Floor = address.Floor,
                    GovernorateShippingCost = _mapper.Map<GovernorateShippingCost>(address.GovernrateShippingCostDto),
                    Street = address.Street,
                    IsDeleted = address.IsDeleted,
                };
                addresses.Add(add);

            }
            var user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Gender = registerDTO.Gender,
                PhoneNumber = registerDTO.PhoneNumber,
                DateOfBirth = registerDTO.DateOfBirth,
                Addresses = addresses,
                UserName = registerDTO.Email
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, registerDTO.Password);
            if (userResult.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync(userRole);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(userRole));
                }

                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, userRole);
                if (roleResult.Succeeded)
                {
                    return ServiceResult.Ok("User Created Successesfuly");
                }
                return ServiceResult.Fail("Fialing during registeration");
            }
            return ServiceResult.Fail("Fialing during registeration");
        }







        // Reset Paasword //

        public bool RequestPasswordReset(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return false;

            // توليد توكن مؤقت (مثلاً GUID)
            string resetToken = Guid.NewGuid().ToString();

            // خزنه مع اليوزر (لو مش مخزن، أضف حقل في جدول المستخدمين أو جدول خاص)
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            _context.SaveChanges();

            //هنا ترسل إيميل فيه الرابط: https://yourfrontend.com/reset-password?token=resetToken
            SendResetEmail(user.Email, resetToken);

            return true;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token
                                                    && u.ResetPasswordExpiry > DateTime.Now);
            if (user == null) return false;

            var hashedpass = new PasswordHasher<ApplicationUser>().HashPassword(user, newPassword);
            user.PasswordHash = hashedpass;

            // امسح التوكن بعد نجاح العملية
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;

            _context.SaveChanges();

            return true;
        }
        public void SendResetEmail(string toEmail, string token)
        {
            var fromEmail = config["EmailSettings:Email"];
            var fromPassword = config["EmailSettings:Password"];
            var resetLink = $"https://localhost:5194/htmlStaticFiles/RegisterationPages/ResetPassword.htmlResetPassword.html?token={token}"; // غير اللينك حسب موقعك

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Reset Your Password",
                Body = $"Click the link to reset your password: {resetLink}",
                IsBodyHtml = true
            };

            smtpClient.Send(message);
        }
        public async Task SaveCodeAndTokenAsync(string email, string code, string token)
        {
            var old = await _context.PasswordResetCodes.FirstOrDefaultAsync(x => x.Email == email);
            if (old != null)
                _context.PasswordResetCodes.Remove(old);

            var record = new PasswordResetCode
            {
                Email = email,
                Code = code,
                Token = token,
                ExpirationTime = DateTime.Now.AddMinutes(15),
                IsUsed = false
            };

            _context.PasswordResetCodes.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateCodeAsync(string email, string code)
        {
            var record = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(x => x.Email == email && x.Code == code && x.ExpirationTime > DateTime.Now && !x.IsUsed);

            return record != null;
        }
        public async Task<string?> GetStoredResetTokenAsync(string email)
        {
            var record = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(x => x.Email == email && x.ExpirationTime > DateTime.Now && !x.IsUsed);

            return record?.Token;
        }

        public async Task DeleteCodeAsync(string email)
        {
            var record = await _context.PasswordResetCodes.FirstOrDefaultAsync(x => x.Email == email);
            if (record != null)
            {
                _context.PasswordResetCodes.Remove(record);
                await _context.SaveChangesAsync();
            }
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));

            var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = string.IsNullOrWhiteSpace(subject) ? "(No Subject)" : subject,
                Body = string.IsNullOrWhiteSpace(body) ? "(No Body)" : body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }


        public async Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }




    }
}
