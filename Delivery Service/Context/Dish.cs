using System;
using System.Collections.Generic;

namespace Delivery_Service.Context;

public partial class Dish
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double? Price { get; set; }

    public string? Description { get; set; }

    public bool? IsVegetarian { get; set; }

    public string? Photo { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
