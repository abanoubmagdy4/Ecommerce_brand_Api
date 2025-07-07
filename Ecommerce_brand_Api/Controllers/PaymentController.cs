
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
        public async Task<IActionResult> Checkout(OrderDTO orderDto)
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



        [HttpPost("Request-order-refund")]
        //[Authorize]
        public async Task<IActionResult> RequestOrderRefund([FromBody] OrderRefundDto dto)
        {
            ServiceResult serviceResult = await _paymentService.HandleOrderRequestRefundAsync(dto);

            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }

        [HttpPost("Request-product-refund")]
        //[Authorize]
        public async Task<IActionResult> RequestProductRefund([FromBody] ProductRefundDto dto)
        {
            ServiceResult serviceResult = await _paymentService.HandleProductRequestRefundAsync(dto);

            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }

        [HttpPost("admin/approve-Order-refund")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrderRefund(ApproveOrderRefundDto dto)
        {

            ServiceResult serviceResult = await _paymentService.HandleApproveOrderRefund(dto);
            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }

        [HttpPost("admin/approve-Product-refund")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProductRefund(ApproveProductRefundDto dto)
        {

            ServiceResult serviceResult = await _paymentService.HandleApproveProductRefund(dto);
            if (serviceResult.Success)
            {
                return Ok(serviceResult.SuccessMessage);
            }
            else
            {
                return BadRequest(serviceResult.ErrorMessage);
            }
        }





    }
}

