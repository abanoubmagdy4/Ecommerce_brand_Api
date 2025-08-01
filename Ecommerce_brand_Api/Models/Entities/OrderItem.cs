﻿namespace Ecommerce_brand_Api.Models.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderId { get; set; }
        public int ProductSizeId { get; set; }

        public ProductSizes productSize { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public ProductRefund productRefund { get; set; }    
    }
}
