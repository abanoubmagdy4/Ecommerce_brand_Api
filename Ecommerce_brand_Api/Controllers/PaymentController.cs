
using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.ErrorHandling;
using Ecommerce_brand_Api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Ecommerce_brand_Api.Controllers
{

    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IServiceUnitOfWork _serviceunitOfWork;
        private readonly IBaseService<Address> _AddressbaseService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;   
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentController> _logger;
        private readonly IWebHostEnvironment _env;
        public PaymentController(IServiceUnitOfWork serviceUnitOfWork, IHttpClientFactory httpClientFactory, ILogger<PaymentController> logger, IWebHostEnvironment env)
        {
            _serviceunitOfWork = serviceUnitOfWork;
            _orderService = _serviceunitOfWork.Orders;
            _AddressbaseService = _serviceunitOfWork.GetBaseService<Address>();
            _userService = _serviceunitOfWork.Users;
            _httpClient = httpClientFactory.CreateClient();
            _paymentService = serviceUnitOfWork.Payment;
            _logger = logger;
            _env = env;
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Any())
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    Message = "Validation failed.",
                    Errors = errors
                });
            }

            try
            {
                var result = await _paymentService.HandleCheckoutAsync(orderDto);

                if (!result.Success)
                {
                    if (result.Data != null)
                        return StatusCode(500, result.Data);

                    return BadRequest(new
                    {
                        Message = result.ErrorMessage ?? "Checkout failed."
                    });
                }

                return Ok(result.Data); 
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Could not reach payment service.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();

                // ✅ طباعة الـ Raw JSON في اللوج
                _logger.LogInformation("🔔 Raw Webhook:\n{Json}", rawBody);

                // ✅ حفظ نسخة من الـ JSON في ملف داخل Logs
                var logFileName = $"webhook_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var logDirectory = Path.Combine(_env.ContentRootPath, "Logs");
                var logFilePath = Path.Combine(logDirectory, logFileName);

                Directory.CreateDirectory(logDirectory);
                await System.IO.File.WriteAllTextAsync(logFilePath, rawBody);

                // ✅ نحاول نعمل Deserialize
                WebhookRequestDto? request = null;
                try
                {
                    request = JsonSerializer.Deserialize<WebhookRequestDto>(rawBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (request == null || request.obj == null)
                    {
                        _logger.LogWarning("⚠️ Webhook payload is null or incomplete.");
                        return BadRequest("Invalid payload");
                    }

                    _logger.LogInformation("✅ Deserialized Successfully. Type = {Type}", request.type);

                    // ✅ معالجة الطلب
                    await _paymentService.HandleIncomingTransactionAsync(request.obj);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Failed to deserialize WebhookRequestDto");
                    return BadRequest("Deserialization failed");
                }

                return Ok(new { status = "saved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while handling webhook");
                return BadRequest();
            }
        }



        [HttpPost("request-refund")]
        //[Authorize]
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestDto dto)
        {
            ServiceResult serviceResult = await _paymentService.HandleRequestRefundAsync(dto);

            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }

      [HttpPost("admin/approve-refund")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRefund(ApproveRefundDto dto)
        {

            ServiceResult serviceResult = await _paymentService.HandleApproveRefund(dto);
            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }

            //public async Task<IActionResult> AdminRefundRequest(int transactionId, decimal ammount)
            //{
            //    amount_cent = ammount * 100 ,
            //    var client = new HttpClient();
            //    var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/void_refund/refund");
            //    request.Headers.Add("Authorization", "Token egy_sk_test_1ab1bc5322ab7aacbd7f24d4656158090110eceb3637028cd5ffc57ea1f5ab4c");
            //    var content = new StringContent("{\"transaction_id\": \"308942574\", \"amount_cents\": \"400\"}", null, "application/json");
            //    request.Content = content;
            //    var response = await client.SendAsync(request);
            //    response.EnsureSuccessStatusCode();
            //    Console.WriteLine(await response.Content.ReadAsStringAsync());

            //}


        }
}

