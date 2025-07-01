using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Helpers.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Created,            // الطلب اتعمل ولسه مفيش دفع
        AwaitingPayment,    // مستني الدفع يخلص
        Processing,         // الدفع تم وجاري التجهيز
        Cancelled,          // الطلب اتلغى قبل ما يتشحن
        Returned            // العميل رجّع الطلب واسترد فلوسه
    }
}
