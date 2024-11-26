namespace WebBanHang.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order order { get; set; }
        public Address address { get; set; }
        public List<DetailsProduct> detailsProducts { get; set; }
        public string userName { get; set; }
    }

    public class DetailsProduct
    {
        public string image { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
