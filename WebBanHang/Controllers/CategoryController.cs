using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
