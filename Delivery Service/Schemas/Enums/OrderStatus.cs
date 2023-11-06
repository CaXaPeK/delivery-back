﻿using System.Text.Json.Serialization;

namespace Delivery_Service.Schemas.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        InProcess,
        Delivered
    }
}
