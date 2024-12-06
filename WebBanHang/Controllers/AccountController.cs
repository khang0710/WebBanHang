using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.ViewModels;
using WebBanHang.Repository;

namespace WebBanHang.Controllers
{
    public class AccountController : Controller
    {
        SnackStoreContext db = new SnackStoreContext();

        public IActionResult Login(string URL)
        {
            return View(new LoginViewModel { Url = URL});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == login.Email);
                if (user == null || user.Password != login.Password)
                {
                    TempData["SuccessMessage"] = "Sai Email hoặc Mật khẩu!";
                    return RedirectToAction("Login");
                }

                // Lưu thông tin vào session/cookie
                HttpContext.Session.SetJson("User", user);

                return RedirectToAction("Index","Home");

            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                return View(user);
            }

            user.Role = "Customer";

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("","Tạo tài khoản thất bại!");
            return View(user);
        }


        [HttpGet]
        public IActionResult Profile()
        {
            var user = HttpContext.Session.GetJson<User>("User");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }
            int id = user.UserId;
            List<CartItemModel> items = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            List<Address> addresses = db.Addresses.Where(x => x.UserId == id).ToList();
            var orders = db.Orders
            .Where(o => o.Address.UserId == id) // Lọc theo UserId nếu cần
            .OrderByDescending(o => o.OrderDate) // Sắp xếp ngày mới nhất trước
            .Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                TotalPrice = o.TotalAmount,
                TotalQuantity = o.OrderDetails.Sum(od => od.Quantity),
                ProductImage = o.OrderDetails
                    .Select(od => od.Product.Image)
                    .FirstOrDefault(),
                ProductName = o.OrderDetails
                    .Select(od => od.Product.Name)
                    .FirstOrDefault(),
                ProductPrice = o.OrderDetails
                    .Select(od => od.Product.Price)
                    .FirstOrDefault(),
                ProductQuantity = o.OrderDetails.
                    Select(od => od.Quantity)
                    .FirstOrDefault(),
                Status = o.Status,
                Payment = o.Payment,
            })
            .ToList();

            CartItemViewModel model = new()
            {
                UserId = id,
                Items = items,
                GrandTotal = items.Sum(x => x.Quantity * x.Price),
                NewAddress = new Address(),
                NewOrder = new Order(),
                Addresses = addresses,
                orderView = orders

            };
            return View(model);
        }

        public IActionResult ChiTietDonHang(string id)
        {
            var user = HttpContext.Session.GetJson<User>("User");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin đơn hàng
            var or = db.Orders.FirstOrDefault(o => o.OrderId == id);
            if (or == null) return NotFound("Order not found."); // Kiểm tra null

            // Lấy thông tin địa chỉ
            var add = db.Addresses.FirstOrDefault(a => a.AddressId == or.AddressId);
            if (add == null) return NotFound("Address not found."); // Kiểm tra null

            // Lấy tên người dùng
            var usn = db.Users
                .Where(u => u.UserId == add.UserId) // Lọc theo UserId từ Address
                .Select(u => u.Username)           // Lấy Username
                .FirstOrDefault() ?? "Unknown User"; // Giá trị mặc định nếu null

            // Lấy danh sách sản phẩm
            var detailsPro = db.OrderDetails
                .Where(d => d.OrderId == id)
                .Select(item => new DetailsProduct
                {
                    image = item.Product.Image ?? "default-image.jpg", // Giá trị mặc định
                    name = item.Product.Name ?? "Unknown Product",
                    price = item.Product.Price,                       // Đảm bảo không null
                    quantity = item.Quantity
                })
                .ToList();

            // Tạo ViewModel
            var model = new OrderDetailsViewModel
            {
                order = or,
                address = add,
                detailsProducts = detailsPro,
                userName = usn
            };

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
           
            return RedirectToAction("Login");
        }


    }
}
