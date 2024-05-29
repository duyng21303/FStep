using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int? AuthorId { get; set; }

    public string? Name { get; set; }

    public virtual Author? Author { get; set; }
}
