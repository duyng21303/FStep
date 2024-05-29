using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class Person
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string? FirstName { get; set; }

    public int? Age { get; set; }

    public string? City { get; set; }
}
