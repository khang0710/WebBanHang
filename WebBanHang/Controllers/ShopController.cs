using System.Collections.Generic;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using X.PagedList;

namespace WebBanHang.Controllers
{
    public class ShopController : Controller
    {
        SnackStoreContext db = new SnackStoreContext();

        public IActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsp = db.Products.AsNoTracking();
            PagedList<Product> list = new PagedList<Product>(listsp, pageNumber, pageSize);
            return View(list);
        }

        public IActionResult SanPhamTheoLoai(int id, int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsp = db.Products.Where(x => x.CategoryId == id).ToList();
            PagedList<Product> list = new PagedList<Product>(listsp, pageNumber, pageSize);
            return View(list);
        }

        public IActionResult ChiTietSP(int id)
        {
            List<Product> list = db.Products.Where(x => x.ProductId == id).ToList();
            return View(list);
        }

        public IActionResult Edit(int id)
        {
            var sp = db.Products.Find(id);
            return View(sp);
        }

        public IActionResult Search(string searchString, int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var products = from p in db.Products
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString)); // Tìm kiếm theo tên sản phẩm
            }

            //return View(products.ToList());
            PagedList<Product> list = new PagedList<Product>(products.ToList(), pageNumber, pageSize);
            return View(list);
        }
    }
}
