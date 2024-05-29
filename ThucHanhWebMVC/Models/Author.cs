using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
