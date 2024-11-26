namespace WebBanHang.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total {
            get { return (Quantity * Price); }
        }
        public string Image {  get; set; }
        public CartItemModel(){}

        public CartItemModel(Product product)
        {
            ProductId = product.ProductId;
            Name = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Image;
        }
    }
}
