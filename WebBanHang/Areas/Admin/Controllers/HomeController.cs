using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.ViewModels;
using WebBanHang.Repository;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]  // Xác định rằng controller này thuộc khu vực Admin
    [Route("admin")]  // Định nghĩa route cho controller và action
    public class HomeController : Controller
    {
        SnackStoreContext db = new SnackStoreContext();

        [Route("")]
        [Route("Index")]  // Định nghĩa route cho action Index
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            return View();
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string URL)
        {
            return View(new LoginViewModel { Url = URL });
        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == login.Email);
                if (user == null || user.Password != login.Password || user.Role == "Customer")
                {
                    TempData["SuccessMessage"] = "Sai Email hoặc Mật khẩu!";
                    return RedirectToAction("Login");
                }

                // Lưu thông tin vào session/cookie
                HttpContext.Session.SetJson("Admin", user);

                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Login");
        }
    }
}
