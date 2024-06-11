using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Product
{
    public int IdProduct { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public float? Price { get; set; }

    public DateTime? ReceivedSellerDate { get; set; }

    public DateTime? SentBuyerDate { get; set; }

    public string? Status { get; set; }

    public string? Detail { get; set; }

    public string? RecieveImg { get; set; }

    public string? SentImg { get; set; }

    public string? ItemLocation { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
