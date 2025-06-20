﻿namespace Ecommerce_brand_Api.Controllers.ResponseWrapper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
