﻿using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Product
{
    public int IdProduct { get; set; }

    public int? Quantity { get; set; }

    public float? Price { get; set; }

    public DateTime? ReceivedSellerDate { get; set; }

    public DateTime? SentBuyerDate { get; set; }

    public string? Status { get; set; }
<<<<<<< HEAD
=======

    public string? Detail { get; set; }
>>>>>>> develop

    public string? RecieveImg { get; set; }

    public string? SentImg { get; set; }

    public string? ItemLocation { get; set; }

<<<<<<< HEAD
    public int? SoldQuantity { get; set; }

=======
>>>>>>> develop
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
