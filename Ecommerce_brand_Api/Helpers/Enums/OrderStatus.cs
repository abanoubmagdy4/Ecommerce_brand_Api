﻿using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Helpers.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Canceled = 5
    }
}
