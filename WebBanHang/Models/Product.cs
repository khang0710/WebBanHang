using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Repository;

namespace WebBanHang.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Image { get; set; }

    public bool Status { get; set; }


    [NotMapped]
    [FileExtension]
    public IFormFile? ImageUpLoad { get; set; }

    public virtual Category? Category { get; set; }

    //public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

//    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
