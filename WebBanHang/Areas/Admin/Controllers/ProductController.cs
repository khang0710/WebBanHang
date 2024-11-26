using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using X.PagedList;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Design;
using Microsoft.AspNetCore.Hosting;
using WebBanHang.Repository;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")] // Định nghĩa khu vực Admin
    [Route("admin/[controller]/")] // Route mặc định cho controller và action
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        SnackStoreContext db = new SnackStoreContext();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

        }

        [Route("")]
        [Route("Index")] // Định nghĩa route cho action Index
        public IActionResult Index(int? page)
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }

            int pageSize = 6; // Số sản phẩm mỗi trang
            int pageNumber = page == null || page < 1 ? 1 : page.Value; // Số trang hiện tại
            var listsp = db.Products.AsNoTracking().Include(p => p.Category).OrderByDescending(x => x.CreatedAt);
            PagedList<Product> list = new PagedList<Product>(listsp, pageNumber, pageSize);
            return View(list); // Trả về danh sách sản phẩm với phân trang
        }


        [Route("DanhMuc")] // Định nghĩa route cho action DanhMuc
        public IActionResult DanhMuc()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }

            var listdm = db.Categories.ToList();
            return View(listdm); // Trả về danh sách danh mục
        }

        // Hiển thị form thêm sản phẩm
        [HttpGet]
        [Route("CreateProduct")]
        public IActionResult CreateProduct()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            ViewBag.Category = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            return View(); // Trả về view để thêm sản phẩm
        }

        // Xử lý thêm sản phẩm
        [HttpPost]
        [Route("CreateProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageUpLoad != null)
                {
                    // Tạo đường dẫn thư mục để lưu trữ hình ảnh
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "img/upload");

                    // Tạo tên file duy nhất bằng GUID kết hợp với tên file gốc
                    string imageName = Path.GetFileName(product.ImageUpLoad.FileName);

                    // Tạo đường dẫn đầy đủ để lưu file
                    string filePath = Path.Combine(uploadsDir, imageName);

                    // Mở file stream để lưu file vào thư mục
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        // Sao chép nội dung file từ ImageUpload vào stream
                        await product.ImageUpLoad.CopyToAsync(fs);
                    }

                    // Gán tên file vào trường Image trong Product
                    product.Image = imageName;
                }


                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost]
        [Route("DeleteProduct/{id}")] // Route cho action Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Tìm sản phẩm theo id
            var product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về NotFound
            }

            // Xóa sản phẩm
            db.Products.Remove(product);
            await db.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
        }


        //[HttpPost]
        [Route("EditProduct/{id}")]
        //[ValidateAntiForgeryToken]
        public IActionResult EditProduct(int id)
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            ViewBag.Category = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            var sp = db.Products.Find(id);
            return View(sp);
        }

        [HttpPost]
        [Route("EditProduct/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Category = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);

                var old_product = db.Products.Find(product.ProductId);
                
                    if (product.ImageUpLoad != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "img/upload");

                        string imageName = Path.GetFileName(product.ImageUpLoad.FileName);

                        string filePath = Path.Combine(uploadsDir, imageName);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageUpLoad.CopyToAsync(fs);
                        }

                        old_product.Image = imageName;
                    }

                    // Cập nhật thông tin sản phẩm
                    old_product.Name = product.Name;
                    old_product.Price = product.Price;
                    old_product.Description = product.Description;
                    old_product.CategoryId = product.CategoryId;
                    old_product.StockQuantity = product.StockQuantity;
                    old_product.Status = product.Status;

                    // Cập nhật cơ sở dữ liệu
                    db.Update(old_product);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
               
            }
            ViewBag.Category = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);

            return View(product);
        }

        [Route("ChiTietSP/{id}")]
        public IActionResult ChiTietSP(int id)
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            var product = db.Products
                          .Include(p => p.Category) // Nếu có liên kết với bảng khác
                          .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }

            return View(product);
        }
    }
}
