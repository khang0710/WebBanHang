namespace WebBanHang.Models.ViewModels
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }              // Mã đơn hàng
        public decimal TotalPrice { get; set; }       // Tổng giá trị đơn hàng
        public int TotalQuantity { get; set; }        // Tổng số lượng sản phẩm
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string Status { get; set; }
        public DateTime? Created { get; set; }
        public string? CustomerName { get; set; }
        public string? Payment { get; set; }
    }
}
