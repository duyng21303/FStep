using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Confirm
{
    public int IdConfirm { get; set; }

    public string? IdUserConfirm { get; set; }

    public string? IdUserConnect { get; set; }
    public int? IdUser { get; set; }


    public bool? Confirm1 { get; set; }

    public int? IdPost { get; set; }

    public int? IdComment { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

    public virtual Post? IdPostNavigation { get; set; }
}
