﻿namespace Ecommerce_brand_Api.Models.Entities
{
    public class ProductImagesPaths
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int Priority { get; set; } = 0;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
