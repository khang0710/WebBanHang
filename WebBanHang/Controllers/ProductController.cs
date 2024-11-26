using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
