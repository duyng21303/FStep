using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Confirm
{
    public int IdConfirm { get; set; }

    public int? IdUser { get; set; }

    public bool? Confirm1 { get; set; }

    public int? IdPost { get; set; }

    public virtual Post? IdPostNavigation { get; set; }
}
