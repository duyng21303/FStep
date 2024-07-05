using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class HistoryPoint
{
    public int IdPoint { get; set; }

    public int? MinusPoint { get; set; }

    public string? Content { get; set; }

    public int? AddPoint { get; set; }

    public DateTime? Date { get; set; }

    public string IdUser { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
