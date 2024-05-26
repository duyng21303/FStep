using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Feedback
{
    public int IdFeedback { get; set; }

    public string? Content { get; set; }

    public int? Rating { get; set; }

    public string IdUser { get; set; } = null!;

    public int IdPost { get; set; }

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
