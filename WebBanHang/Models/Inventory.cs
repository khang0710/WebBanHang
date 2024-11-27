using System;
using System.Collections.Generic;

namespace WebBanHang.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    public string? WarehouseLocation { get; set; }

    public int Quantity { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
}
