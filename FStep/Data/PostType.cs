using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class PostType
{
    public int IdType { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
