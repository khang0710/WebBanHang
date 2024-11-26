using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public string OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
