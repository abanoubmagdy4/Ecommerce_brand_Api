using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class ExtrasContainerDTO
    {
        [JsonPropertyName("creation_extras")]
        public CreationExtrasDTO Creation_Extras { get; set; }

        [JsonPropertyName("confirmation_extras")]
        public object Confirmation_Extras { get; set; }
    }
}
