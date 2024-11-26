using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string OrderId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
