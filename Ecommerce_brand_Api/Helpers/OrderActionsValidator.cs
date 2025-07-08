namespace Ecommerce_brand_Api.Helpers
{
    public static class OrderActionsValidator
    {
        /// <summary>
        /// Determines whether the order can be cancelled based on order, payment, and shipping status.
        /// </summary>

        public static bool CanCancel(PaymentStatus paymentStatus, ShippingStatus shippingStatus, DateTime orderDate)
        {
            // مش هينفع نلغي لو خلاص حصل Refund أو Cancel
            if (paymentStatus == PaymentStatus.Refunded || paymentStatus == PaymentStatus.Canceled)
                return false;

            // Authorized but not captured → ينفع نعمل Void (لو في نفس اليوم ولسه مش متشحن)
            if (paymentStatus == PaymentStatus.Authorized
                && orderDate.Date == DateTime.Now.Date
                && shippingStatus == ShippingStatus.NotShipped)
                return true;

            // Captured → ينفع نعمل Refund لو لسه مش متشحن
            if (paymentStatus == PaymentStatus.Success
                && (shippingStatus == ShippingStatus.NotShipped
                    || shippingStatus == ShippingStatus.ReadyToShip
                    || shippingStatus == ShippingStatus.NotApplicable))
                return true;

            // Pending → لسه تحت الانتظار، ينفع نلغي
            if (paymentStatus == PaymentStatus.Pending)
                return true;

            // Declined أو أي حاجة تانية → منلغيهاش
            return false;
        }

        /// <summary>
        /// Determines whether the order can be refunded based on order, payment, and shipping status.
        /// Refunds are only allowed if the payment was captured (Success) and the order is not already returned or cancelled.
        /// </summary>
        public static bool CanRefund(OrderStatus orderStatus, PaymentStatus paymentStatus, ShippingStatus shippingStatus)
        {
            // Refund only applies to captured payments
            if (paymentStatus != PaymentStatus.Success)
                return false;

            // Cannot refund if order is already returned or cancelled
            if (orderStatus == OrderStatus.Returned || orderStatus == OrderStatus.Cancelled)
                return false;

            // Refund allowed if order is processing (regardless of shipping status)
            if (orderStatus == OrderStatus.Processing)
                return true;

            // Refund allowed if the order was delivered but not marked as returned yet
            if (shippingStatus == ShippingStatus.Delivered)
                return true;

            // Refund allowed if delivery failed
            if (shippingStatus == ShippingStatus.DeliveryFailed)
                return true;

            // In all other cases, refund is not allowed
            return false;
        }
    }

}
