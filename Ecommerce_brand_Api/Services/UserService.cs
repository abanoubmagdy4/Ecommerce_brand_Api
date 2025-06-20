using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Models.Entities;
using Microsoft.Extensions.Options;
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

        public UserService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           ITokenService tokenService,
                           AppDbContext context,
                           IConfiguration config,
                           IOptions<EmailSettings> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            this.config = config;
            _emailSettings = options.Value;
        }


        public async Task<ServiceResult> LoginAsync([FromForm] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return ServiceResult.Fail("Invalid Email or Password");

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                return ServiceResult.Fail("Invalid Email or Password");

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            // Generate JWT Token
            string token = _tokenService.CreateToken(user, userRoles);

            return ServiceResult.Ok(token);

            // Remember Me !!!!!!!!!!
        }


        public async Task<ServiceResult> RegisterAsync([FromForm] RegisterDto registerDTO, string userRole)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
                return ServiceResult.Fail("Email ALready Exists !");

             List<Address> addresses = new List<Address>(); 
         
            foreach(var address in registerDTO.Addresses) {
                Address add = new Address()
                {
                    Apartment = address.Apartment,
                    Building = address.Building,
                    City = address.City,
                    Country = address.Country,
                    Floor = address.Floor,
                    State = address.State,
                    Street = address.Street,
                    IsDeleted = address.IsDeleted,
                };
                addresses.Add(add);

            }
            var user = new ApplicationUser()
            {
                Email = registerDTO.Email.Split("@")[0],
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Gender = registerDTO.Gender,
                PhoneNumber = registerDTO.PhoneNumber,
                DateOfBirth = registerDTO.DateOfBirth,
                Addresses = addresses
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, registerDTO.Password);
            if (userResult.Succeeded)
            {
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

        public async Task SaveCodeAsync(string email, string code)
        {
            // امسح الأكواد القديمة الغير مستخدمة للإيميل ده (اختياري، للتنضيف)
            var oldCodes = _context.PasswordResetCodes
                .Where(p => p.Email == email && !p.IsUsed);
            _context.PasswordResetCodes.RemoveRange(oldCodes);

            var resetCode = new PasswordResetCode
            {
                Email = email,
                Code = code,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

            await _context.PasswordResetCodes.AddAsync(resetCode);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ValidateCodeAsync(string email, string code)
        {
            var match = await _context.PasswordResetCodes
                .Where(p => p.Email == email &&
                            p.Code == code &&
                            !p.IsUsed &&
                            p.ExpirationTime > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (match == null)
                return false;

            match.IsUsed = true; // علم الكود إنه اتستخدم
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteCodeAsync(string email)
        {
            var codes = await _context.PasswordResetCodes
                .Where(p => p.Email == email)
                .ToListAsync();

            _context.PasswordResetCodes.RemoveRange(codes);
            await _context.SaveChangesAsync();
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

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var code = new Random().Next(100000, 999999).ToString();

            var resetCode = new PasswordResetCode
            {
                Email = user.Email,
                Code = code,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10)
            };

            _context.PasswordResetCodes.Add(resetCode);
            await _context.SaveChangesAsync();

            return code;
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
