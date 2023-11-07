using System;
using System.Collections.Generic;

namespace Delivery_Service.Context;

public partial class DishInCart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int DishId { get; set; }

    public int? OrderId { get; set; }

    public int Count { get; set; }
}
