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

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
           
            return RedirectToAction("Login");
        }


    }
}
