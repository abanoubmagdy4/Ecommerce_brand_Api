using Ecommerce_brand_Api.Models.Dtos;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace Ecommerce_brand_Api.Controllers
{
    

        [Route("api/paymob")]
        [ApiController]
        public class PaymentController : ControllerBase
        {

        //[HttpPost("checkout")]
        //public async Task<IActionResult> Checkout([FromBody] CartDto cartDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState
        //            .Where(e => e.Value.Errors.Any())
        //            .ToDictionary(
        //                e => e.Key,
        //                e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
        //            );

        //        return BadRequest(new
        //        {
        //            Message = "Validation failed.",
        //            Errors = errors
        //        });
        //    }

        //    try
        //    {
        //        var httpClient = new HttpClient();

        //        var url = "https://accept.paymob.com/v1/intention/";

        //        var payload = new
        //        {
        //            amount = 40000,
        //            currency = "EGP",
        //            payment_methods = new[] { 5145466, 5145604, 5145468 },
        //            items = cartDto.CartItems.Select(i => new
        //            {
        //                name = i.namr,
        //                amount = i.TotalPrice,
        //                description = i.p,
        //                quantity = i.Quantity
        //            }),
        //            billing_data = new
        //            {
        //                apartment = "5",
        //                first_name = cartDto.CustomerFirstName,
        //                last_name = cartDto.CustomerLastName,
        //                street = "10 El Tahrir St",
        //                building = "25",
        //                phone_number = cartDto.PhoneNumber,
        //                city = cartDto.City,
        //                country = cartDto.Country,
        //                email = cartDto.BillingEmail,
        //                floor = "3",
        //                state = cartDto.City
        //            },
        //            customer = new
        //            {
        //                first_name = cartDto.CustomerFirstName,
        //                last_name = cartDto.CustomerLastName,
        //                email = cartDto.CustomerEmail,
        //                extras = new { order_source = "website" }
        //            },
        //            extras = new { notes = "Test order from backend" }
        //        };

        //        var json = JsonSerializer.Serialize(payload);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        httpClient.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("", "");

        //        var response = await httpClient.PostAsync(url, content);

        //        if (!response.IsSuccessStatusCode)
        //        {                      
        //            var errorBody = await response.Content.ReadAsStringAsync();
        //            return StatusCode((int)response.StatusCode, new
        //            {
        //                Message = "Payment order intention call failed.",
        //                Status = response.StatusCode,
        //                Details = errorBody
        //            });
        //        } 

        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        var jsonDoc = JsonDocument.Parse(responseBody);
        //        var root = jsonDoc.RootElement;

        //        if (root.TryGetProperty("client_secret", out JsonElement secretElement))
        //        {
        //            var clientSecret = secretElement.GetString();

        //            var publicKey = "egy_pk_test_Wzpx5ANqdUPO28zxGtNd6HqkxiQMTFVc";
                          
        //            ///add payment model to db
        //            var checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

                    
        //            return Ok(new
        //            {
        //                Message = "Payment intention created.",
        //                Url = checkoutUrl
        //            });
        //        }
        //        else
        //        {
        //            return StatusCode(500, new
        //            {
        //                Message = "client_secret not found in response.",
        //                Response = responseBody
        //            });
        //        }




        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Message = "Could not reach payment service.",
        //            Details = ex.Message
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Message = "An unexpected error occurred.",
        //            Details = ex.Message
        //        });
        //    }
        //}


        [HttpPost("webhook")]
            public IActionResult Webhook([FromBody] object payload)
            {
            System.IO.File.AppendAllText("webhook_log.txt", $"[{DateTime.Now}] Payload: {payload}\n");

            return Ok();
            }
        }
    }

