using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class Category
{
    public int CategoriesId { get; set; }

    public string CategoriesName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
