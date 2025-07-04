namespace Ecommerce_brand_Api.Helpers
{
    public static class OrderActionsValidator
    {
        /// <summary>
        /// Determines whether the order can be cancelled based on order, payment, and shipping status.
        /// </summary>
        public static bool CanCancel(OrderStatus orderStatus, PaymentStatus paymentStatus, ShippingStatus shippingStatus)
        {
            // Case 1: Order just created and not yet processed or paid
            if (orderStatus == OrderStatus.Created)
                return true;

            // Case 2: Awaiting payment and the payment is still pending
            if (orderStatus == OrderStatus.AwaitingPayment && paymentStatus == PaymentStatus.Pending)
                return true;

            // Case 3: Order is being processed, payment is authorized but not captured yet, and shipment hasn't gone out
            if (orderStatus == OrderStatus.Processing
                && paymentStatus == PaymentStatus.Authorized
                && (shippingStatus == ShippingStatus.NotShipped
                    || shippingStatus == ShippingStatus.ReadyToShip
                    || shippingStatus == ShippingStatus.NotApplicable)) 
                return true;

            // If none of the above, the order cannot be cancelled
            return false;
        }

        /// <summary>
        /// Determines whether the order can be refunded based on order, payment, and shipping status.
        /// </summary>
        public static bool CanRefund(OrderStatus orderStatus, PaymentStatus paymentStatus, ShippingStatus shippingStatus)
        {
            // Refund is only allowed if the payment was successful
            if (paymentStatus != PaymentStatus.Success)
                return false;

            // Case 1: Order is processing and has not shipped yet or is ready to ship
            if (orderStatus == OrderStatus.Processing &&
                (shippingStatus == ShippingStatus.NotShipped || shippingStatus == ShippingStatus.ReadyToShip))
                return true;

            // Case 2: Order is processing and already shipped or out for delivery
            if (orderStatus == OrderStatus.Processing &&
                (shippingStatus == ShippingStatus.Shipped || shippingStatus == ShippingStatus.OutForDelivery))
                return true;

            // Case 3: Cannot refund if already returned or cancelled
            if (orderStatus == OrderStatus.Returned || orderStatus == OrderStatus.Cancelled)
                return false;

            // Case 4: Order delivered and not marked as returned yet
            if (shippingStatus == ShippingStatus.Delivered && orderStatus != OrderStatus.Returned)
                return true;

            // Case 5: Delivery failed
            if (shippingStatus == ShippingStatus.DeliveryFailed)
                return true;

            // If none of the above conditions are met, refund is not allowed
            return false;
        }
    }

}
