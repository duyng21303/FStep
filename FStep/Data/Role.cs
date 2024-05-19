using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Role
{
    public int IdRole { get; set; }

    public int? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
