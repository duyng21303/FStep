using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Product
{
    public int IdProduct { get; set; }

    public int? Quantity { get; set; }

    public float? Price { get; set; }

    public string? Status { get; set; }

    public string? ItemLocation { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
