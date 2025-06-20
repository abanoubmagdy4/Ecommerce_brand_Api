namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult> LoginAsync(LoginDto loginDTO);
        Task<ServiceResult> RegisterAsync(RegisterDto registerDTO, string userRole);
        bool RequestPasswordReset(string email);
        bool ResetPassword(string token, string newPassword);
        Task DeleteCodeAsync(string email);
        Task<bool> ValidateCodeAsync(string email, string code);
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task SaveCodeAndTokenAsync(string email, string code, string token);
        Task<string?> GetStoredResetTokenAsync(string email);
        
            //Task<bool> CheckEmailExistAsync(string email);
        }
}