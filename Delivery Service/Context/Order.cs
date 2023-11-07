using System;
using System.Collections.Generic;

namespace Delivery_Service.Context;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public TimeOnly? DeliveryTime { get; set; }

    public DateOnly? OrderDate { get; set; }

    public TimeOnly? OrderTime { get; set; }

    public double? Price { get; set; }

    public string? AddressId { get; set; }

    public string? Status { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
