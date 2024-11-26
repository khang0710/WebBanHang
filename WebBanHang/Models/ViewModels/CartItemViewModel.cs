namespace WebBanHang.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int UserId { get; set; }
        public List<CartItemModel> Items { get; set; }
        public decimal GrandTotal { get; set; }
        public List<Address> Addresses { get; set; } // Danh sách địa chỉ
        public Address NewAddress { get; set; }
        public Order NewOrder { get; set; }
        public List<OrderViewModel> orderView { get; set; }
    }
}
