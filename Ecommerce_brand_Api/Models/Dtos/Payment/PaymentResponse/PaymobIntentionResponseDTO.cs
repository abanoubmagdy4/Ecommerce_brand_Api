using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Models.Dtos.Payment.PaymentResponse
{
    public class PaymobIntentionResponseDTO
    {
        [JsonPropertyName("payment_keys")]
        public List<PaymentKeyDTO> PaymentKeys { get; set; }

        [JsonPropertyName("intention_order_id")]
        public long IntentionOrderId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("intention_detail")]
        public IntentionDetailDTO IntentionDetail { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("payment_methods")]
        public List<PaymentMethodDTO> PaymentMethods { get; set; }

        [JsonPropertyName("special_reference")]
        public string SpecialReference { get; set; }

        [JsonPropertyName("extras")]
        public ExtrasContainerDTO Extras { get; set; }

        [JsonPropertyName("confirmed")]
        public bool Confirmed { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("card_detail")]
        public object CardDetail { get; set; }

        [JsonPropertyName("card_tokens")]
        public List<object> CardTokens { get; set; }

        [JsonPropertyName("object")]
        public string ObjectType { get; set; }
    }
}
