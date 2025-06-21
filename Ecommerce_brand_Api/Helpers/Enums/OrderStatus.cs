using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Helpers.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending = 0,          
        Processing = 1,      
        Canceled = 2,        
        Shipped = 3,       
        Delivered = 4,          
        RefundRequested = 5,   
        Refunded = 6
    }
}
