using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Repository;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")] // Định nghĩa khu vực Admin
    [Route("admin/[controller]/")]
    public class UserController : Controller
    {
        SnackStoreContext db = new SnackStoreContext();

        public IActionResult Index()
        {
            return View();
        }

        [Route("DSAdmin")]
        public IActionResult DSAdmin()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            var list = db.Users.Where(u => u.Role == "Staff" || u.Role == "Admin").ToList();
            return View(list);
        }

        [HttpGet]
        [Route("CreateAdmin")]
        public IActionResult CreateAdmin()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [Route("CreateAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("DSAdmin");
            }
            return View(user);
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");

            return RedirectToAction("Login", "Home", new { area = "Admin" });
        }

        [Route("DSKhachHang")]
        public IActionResult DSKhachHang()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            var list = db.Users.Where(u => u.Role == "Customer").ToList();
            return View(list);
        }

        [Route("ChiTietTaiKhoan/{id}")]
        public IActionResult ChiTietTaiKhoan(int id)
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }

            var u = db.Users.FirstOrDefault(u => u.UserId == id);
            return View(u);
        }

        // Xử lý xóa u
        [HttpPost]
        [Route("DeleteUser/{id}")] // Route cho action Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Tìm sản phẩm theo id
            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về NotFound
            }

            // Xóa sản phẩm
            db.Users.Remove(user);
            await db.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
