using Ecommerce_brand_Api.Helpers;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly EmailSettings _emailSettings;
        
        public ContactUsController(
            IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendContactUsEmail([FromBody] ContactUsDto dto)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = dto.Subject,
                    Body = $@"
                Name: {dto.FirstName} {dto.LastName}
                Email: {dto.Email}
                Phone: {dto.Phone}

                Message:
                {dto.Message}
            ",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(_emailSettings.SenderEmail);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                return Ok(new { message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the email.", error = ex.Message });
            }
        }




    }
}
