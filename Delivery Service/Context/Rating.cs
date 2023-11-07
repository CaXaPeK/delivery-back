using System;
using System.Collections.Generic;

namespace Delivery_Service.Context;

public partial class Rating
{
    public int Id { get; set; }

    public double Rating1 { get; set; }

    public int? UserId { get; set; }

    public int? DishId { get; set; }

    public virtual Dish? Dish { get; set; }

    public virtual User? User { get; set; }
}
