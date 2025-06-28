using Ecommerce_brand_Api.Models.Dtos;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;
using System.Linq;
using System.Net.Http;
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
        private readonly HttpClient _httpClient;

        public PaymentController(IServiceUnitOfWork serviceUnitOfWork , IHttpClientFactory httpClientFactory)
        {
            _serviceunitOfWork = serviceUnitOfWork;
            _orderService = _serviceunitOfWork.Orders;
            _AddressbaseService = _serviceunitOfWork.GetBaseService<Address>();
            _userService = _serviceunitOfWork.Users;
            _httpClient = httpClientFactory.CreateClient();

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


                var orderDTOAfterPrepare = await _orderService.BuildOrderDto(orderDto);


                var Ordercreated = await _orderService.AddNewOrderAsync(orderDTOAfterPrepare);

                var shippingAddress = await _AddressbaseService.GetByIdAsync(Ordercreated.ShippingAddressId);
                if (shippingAddress == null)
                    return NotFound(new { Message = "Shipping address not found." });
                var User = await _userService.GetByStringIdAsync(Ordercreated.CustomerId);
                if (User == null)
                    return NotFound(new { Message = "User not found." });

             

                var url = "https://accept.paymob.com/v1/intention/";
                var itemsList = Ordercreated.OrderItems
                       .Select(i => new
                       {
                           name = i.OrderItemId.ToString(),
                           amount = i.TotalPrice,
                           description = $"OrderId {i.OrderItemId} For Product Id {i.ProductId} In Database",
                           quantity = i.Quantity 
                       })
                           .Append(new
                           {
                               name = "Discount",
                               amount = -Ordercreated.DiscountValue,
                               description = "Total discount applied on this order",
                               quantity = 1
                           })
                              .ToList();
                var payload = new
                {
                    amount = Ordercreated.TotalOrderPrice,
                    currency = "EGP",
                    payment_methods = new[] { 5145466, 5145604, 5145468 },
                    items = itemsList,

                    billing_data = new
                    {
                        apartment = shippingAddress.Apartment,
                        first_name = User.FirstName,
                        last_name = User.LastName,
                        street = shippingAddress.Street,
                        building = shippingAddress.Building,
                        phone_number = User.PhoneNumber,
                        city = shippingAddress.City,
                        country = shippingAddress.Country,
                        email = User.Email,
                        floor = shippingAddress.Floor,
                        state = shippingAddress.GovernorateShippingCost.Name,
                    },
                    customer = new
                    {
                        first_name = User.FirstName,
                        last_name = User.LastName,
                        email = User.Email,
                        extras = new { order_source = "website" }
                    },
                    extras = new { notes = "Test order from backend" }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("", "");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        Message = "Payment order intention call failed.",
                        Status = response.StatusCode,
                        Details = errorBody
                    });
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseBody);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("client_secret", out JsonElement secretElement))
                {
                    var clientSecret = secretElement.GetString();

                    var publicKey = "egy_pk_test_Wzpx5ANqdUPO28zxGtNd6HqkxiQMTFVc";

                    ///add payment model to db
                    var checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";


                    return Ok(new
                    {
                        Message = "Payment intention created.",
                        Url = checkoutUrl
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Message = "client_secret not found in response.",
                        Response = responseBody
                    });
                }




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
        public IActionResult Webhook([FromBody] object payload)
        {
            System.IO.File.AppendAllText("webhook_log.txt", $"[{DateTime.Now}] Payload: {payload}\n");

            return Ok();
        }

        //public async Task<IActionResult> ClientRefundRequest(int transactionId, decimal ammount)
        //{


        //}
        //public async Task<IActionResult> ClientCancellationRequest(int transactionId, decimal ammount)
        //{


        //}
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

