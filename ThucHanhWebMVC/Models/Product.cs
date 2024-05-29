using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public short? ModelYear { get; set; }

    public decimal? ListPrice { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
