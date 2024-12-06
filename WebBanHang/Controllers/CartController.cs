using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanHang.Models;
using WebBanHang.Models.ViewModels;
using WebBanHang.Models.VNPay;
using WebBanHang.Repository;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly SnackStoreContext _context;
        SnackStoreContext db = new SnackStoreContext();
        private readonly IVnPayService _vnPayService;

        public CartController(SnackStoreContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            List<CartItemModel> items = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                Items = items,
                GrandTotal = items.Sum(x => x.Quantity * x.Price),
            };
            return View(cartVM);
        }

        public async Task<IActionResult> Add(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            List<CartItemModel> carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = carts.Where(c => c.ProductId == id).FirstOrDefault();

            if (cartItem == null)
            {
                carts.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", carts);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItemModel> carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = carts.Where(c => c.ProductId == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                carts.RemoveAll(c => c.ProductId == id);
            }

            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
            }
            return Redirect(Request.Headers["Referer"].ToString());

        }
        public async Task<IActionResult> Increase(int id)
        {
            List<CartItemModel> carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = carts.Where(c => c.ProductId == id).FirstOrDefault();

            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                carts.RemoveAll(c => c.ProductId == id);
            }

            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
            }
            return Redirect(Request.Headers["Referer"].ToString());

        }

        public async Task<IActionResult> Remove(int id)
        {
            List<CartItemModel> carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = carts.Where(c => c.ProductId == id).FirstOrDefault();
            
            carts.RemoveAll(c => c.ProductId == id);
            
            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
            }
            return Redirect(Request.Headers["Referer"].ToString());

        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1) // Lấy số lượng từ form, mặc định là 1
        {
            // Tìm sản phẩm theo ID
            Product product = await _context.Products.FindAsync(productId);

            // Nếu không tìm thấy sản phẩm, chuyển hướng đến trang lỗi hoặc xử lý lỗi
            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ session, nếu giỏ hàng chưa có thì tạo mới
            List<CartItemModel> carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            CartItemModel cartItem = carts.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm mới với số lượng từ form
                carts.Add(new CartItemModel
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Image = product.Image// Sử dụng số lượng từ form
                });
            }
            else
            {
                // Nếu sản phẩm đã có, cập nhật số lượng
                cartItem.Quantity += quantity; // Cộng thêm số lượng từ form
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", carts);

            // Chuyển hướng về trang trước đó (giữ nguyên trang sản phẩm)
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Checkout(int id)
        {
            var user = HttpContext.Session.GetJson<User>("User");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login","Account");
            }

            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>("Cart");

            // Kiểm tra nếu giỏ hàng rỗng hoặc không tồn tại
            if (cart == null || cart.Count <= 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            List<CartItemModel> items = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            List<Address> addresses = db.Addresses.Where(x => x.UserId == id).ToList();
            CartItemViewModel cartVM = new()
            {
                UserId = id,
                Items = items,
                GrandTotal = items.Sum(x => x.Quantity * x.Price),
                NewAddress = new Address(),
                NewOrder = new Order(),
                Addresses = addresses,
                orderView = new List<OrderViewModel>()
            };
            return View(cartVM);
        }
        
        [HttpPost]
        public IActionResult Address(CartItemViewModel model)
        {
            if (model.NewAddress != null)
            {
                // Lấy NewAddress từ model
                Address newAddress = model.NewAddress;

                // Xử lý lưu vào cơ sở dữ liệu
                db.Addresses.Add(newAddress);
                db.SaveChanges();

                // Chuyển hướng về Checkout
                return Redirect(Request.Headers["Referer"].ToString());
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult CreateOrder(CartItemViewModel model)
        {
            
            if (model.NewOrder != null)
            {
                Order order = model.NewOrder;
                order.Payment = "Thanh toán khi nhận hàng";

                db.Orders.Add(order);
                db.SaveChanges();

                var newOrderId = order.OrderId;
                List<CartItemModel> items = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                foreach (var item in items)
                {
                    var orderdetail = new OrderDetail
                    {
                        OrderId = newOrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };

                    db.OrderDetails.Add(orderdetail);

                    // Giảm số lượng trong bảng sản phẩm
                    var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        if (product.StockQuantity < item.Quantity)
                        {
                            return BadRequest("Số lượng sản phẩm trong kho không đủ.");
                        }
                        product.StockQuantity -= item.Quantity;
                        
                    }
                }
                db.SaveChanges();
                HttpContext.Session.Remove("Cart");
                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("Profile", "Account");
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }

}
