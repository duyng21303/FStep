using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class TOrder
{
    public string Orderid { get; set; } = null!;

    public DateTime? Date { get; set; }

    public double? Total { get; set; }
}
