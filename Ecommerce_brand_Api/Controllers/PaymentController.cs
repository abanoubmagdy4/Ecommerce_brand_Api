﻿
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

        public PaymentController(IServiceUnitOfWork serviceUnitOfWork, IHttpClientFactory httpClientFactory)
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
                ApplicationUser existedUser = await _userService.FindByEmailAsync(orderDto.CustomerInfo.Email);
                if (existedUser == null)
                    return BadRequest("Please Complete Your Information !");

                existedUser = await _userService.UpdatedUserAsync(existedUser, orderDto.CustomerInfo);
                Address newAddress = new Address();

                if (orderDto.AddressInfo.Id == 0)
                {
                    newAddress = await _userService.AddNewAddressAsync(orderDto.AddressInfo, existedUser.Id);
                }
                ServiceResult addressResult = await _userService.UpdatedAddressAsync(orderDto.AddressInfo);
                if (addressResult.Success == false)
                    return BadRequest("Please Complete Your information !");

                newAddress = (Address)addressResult.Data;

                ServiceResult orderDtoServiceResult = await _orderService.BuildOrderDto(orderDto);
                if (!orderDtoServiceResult.Success)
                    return BadRequest(orderDtoServiceResult.ErrorMessage);
                if (orderDtoServiceResult.Data == null)
                    return BadRequest("Order preparation failed: no data returned.");
                OrderDTO orderDTOAfterPrepare =(OrderDTO)orderDtoServiceResult.Data;


                Order orderCreated = await _orderService.AddNewOrderAsync(orderDTOAfterPrepare, newAddress, existedUser);
                if (orderCreated == null)
                    return StatusCode(500, "An error occurred while creating the order.");


                var url = "https://accept.paymob.com/v1/intention/";
                var itemsList = orderCreated.OrderItems
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
                   amount = -orderCreated.DiscountValue,
                   description = "Total discount applied on this order",
                   quantity = 1
               })
               .Append(new
               {
                   name = "Shipping",
                   amount = orderCreated.ShippingCost ?? 0,
                   description = "Shipping cost for this order",
                   quantity = 1
               })
               .ToList();
                var payload = new
                {
                    amount = orderCreated.TotalOrderPrice,
                    currency = "EGP",
                    payment_methods = new[] { 5145466, 5145604, 5145468 },
                    items = itemsList,

                    billing_data = new
                    {
                        apartment = orderCreated.ShippingAddress.Apartment,
                        first_name = orderCreated.FirstName,
                        last_name = orderCreated.LastName,
                        street = orderCreated.ShippingAddress.Street,
                        building = orderCreated.ShippingAddress.Building,
                        phone_number = orderCreated.PhoneNumber,
                        city = orderCreated.ShippingAddress.Street,
                        country = orderCreated.ShippingAddress.Country,
                        email = orderCreated.Customer.Email,
                        floor = orderCreated.ShippingAddress.Floor,
                        state = orderCreated.ShippingAddress.City,
                    },
                    customer = new
                    {
                        first_name = orderCreated.FirstName,
                        last_name = orderCreated.LastName,
                        email = orderCreated.Customer.Email,
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
        public async Task<IActionResult> Webhook([FromBody] WebhookRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid payload");

            var payment = new Payment
            {
                OrderId = request.order_id,
                Currency = request.currency,
                IsPaymentLocked = request.is_payment_locked,
                IsReturn = request.is_return,
                IsCancel = request.is_cancel,
                IsReturned = request.is_returned,
                IsCanceled = request.is_canceled,
                PaidAmountCents = request.paid_amount_cents,
                PaymentStatus = request.payment_status,
                PaymentMethod = request.payment_method,
                SourceType = request.source_data?.type,
                SourcePhoneNumber = request.source_data?.phone_number,
                FirstName = request.payment_key_claims?.billing_data?.first_name,
                LastName = request.payment_key_claims?.billing_data?.last_name,
                Email = request.payment_key_claims?.billing_data?.email,
                Address = $"{request.payment_key_claims?.billing_data?.street}, {request.payment_key_claims?.billing_data?.city}",
                CreatedAt = request.created_at,
                Items = request.items?.Select(i => new PaymentItem
                {
                    Name = i.name,
                    Description = i.description,
                    AmountCents = i.amount_cents,
                    Quantity = i.quantity
                }).ToList()
            };

            //_context.Payments.Add(payment);
            //await _context.SaveChangesAsync();

            return Ok(new { status = "saved" });
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

