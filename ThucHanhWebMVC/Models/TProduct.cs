using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class TProduct
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Unitprice { get; set; }

    public int? Quantity { get; set; }

    public int? Status { get; set; }
}
