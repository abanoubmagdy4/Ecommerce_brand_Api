namespace Ecommerce_brand_Api.Helpers.Enums
{
    public enum ShippingStatus
    {
        NotApplicable,      // لسه الطلب مش هيتشحن (مثلاً ملغي أو رقمي)
        NotShipped,         // لسه ما اتبعتش
        ReadyToShip,        // اتحضّر ومستني شركة الشحن
        Shipped,            // خرج من المخزن
        OutForDelivery,     // في الطريق للعميل
        Delivered,          // اتسلم فعلاً
        DeliveryFailed      // فشل في التوصيل
    }
}
