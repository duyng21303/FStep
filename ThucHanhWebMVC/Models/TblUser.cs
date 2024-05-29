using System;
using System.Collections.Generic;

namespace ThucHanhWebMVC.Models;

public partial class TblUser
{
    public string UserId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Password { get; set; }

    public string? RoleId { get; set; }

    public bool? Status { get; set; }
}
