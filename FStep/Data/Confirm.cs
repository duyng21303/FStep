using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Confirm
{
    public int IdConfirm { get; set; }

<<<<<<< HEAD
    public string? IdUserConfirm { get; set; }

    public string? IdUserConnect { get; set; }
=======
    public int? IdUser { get; set; }
>>>>>>> develop

    public bool? Confirm1 { get; set; }

    public int? IdPost { get; set; }

<<<<<<< HEAD
    public int? IdComment { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

=======
>>>>>>> develop
    public virtual Post? IdPostNavigation { get; set; }
}
