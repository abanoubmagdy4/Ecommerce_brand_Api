
using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.ErrorHandling;
using Ecommerce_brand_Api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Controllers
{

    [Route("api/paymob")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IServiceUnitOfWork _serviceunitOfWork;
        private readonly IBaseService<Address> _AddressbaseService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;   
        private readonly HttpClient _httpClient;

        public PaymentController(IServiceUnitOfWork serviceUnitOfWork, IHttpClientFactory httpClientFactory)
        {
            _serviceunitOfWork = serviceUnitOfWork;
            _orderService = _serviceunitOfWork.Orders;
            _AddressbaseService = _serviceunitOfWork.GetBaseService<Address>();
            _userService = _serviceunitOfWork.Users;
            _httpClient = httpClientFactory.CreateClient();
            _paymentService = serviceUnitOfWork.Payment;

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




        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequestDto request)
        {
            if (request == null || request.obj == null)
                return BadRequest("Invalid payload");

            var transaction = request.obj;

            await _paymentService.HandleIncomingTransactionAsync(transaction);

            return Ok(new { status = "saved" });
        }



        [HttpPost("request-refund")]
        [Authorize]
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
        [Authorize(Roles = "Admin")]
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

