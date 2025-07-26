using Ecommerce_brand_Api.Models.Entities;

namespace Ecommerce_brand_Api.Helpers.Builders
{
    public static class OrderSummaryDtoBuilder
    {
        public static OrderSummaryDto PrepareOrderSummary(Order order, Payment payment)
        {
            return new OrderSummaryDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                TransactionId = payment?.TransactionId,
                CustomerFullName = $"{order.FirstName} {order.LastName}",
                PhoneNumber = order.PhoneNumber,
                CreatedAt = order.CreatedAt,
                DeliveredAt = order.DeliveredAt,
                OrderAddressInfo = order.OrderAddressInfo,
                PaymentStatus = payment?.PaymentStatus,
                PaidAmount = payment?.PaidAmountCents / 100m,
                PaymentMethod = order.paymentMethod.ToString(),
                ShippingCost = order.ShippingCost,
                TotalOrderPrice = order.TotalOrderPrice,
                DiscountValue = order.DiscountValue,
                OrderStatus = order.OrderStatus.ToString(),
                ShippingStatus = order.ShippingStatus.ToString(),
                PaymobOrderId = order.PaymobOrderId,
                PaymobOrderReference = payment?.PaymobOrderId,
                IsRefunded = payment?.IsRefunded ?? false,
                IsCanceled = payment?.IsCanceled ?? false,
                MatchScore = null,
                Items = order.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ProductSizeId = i.ProductSizeId,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }
    }
}
