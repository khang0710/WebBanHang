using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.ViewModels;
using WebBanHang.Repository;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")] // Định nghĩa khu vực Admin
    [Route("admin/[controller]/")]
    public class OrderController : Controller
    {
        SnackStoreContext db = new SnackStoreContext();

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
            }
            List<OrderViewModel> orders = db.Orders
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
               Created = o.OrderDate,
               CustomerName = o.Address.User.Username
           })
           .ToList();
            return View(orders);
        }

        [Route("CapNhatDonHang/{id}")]
        public IActionResult CapNhatDonHang(string id)
        {
            var user = HttpContext.Session.GetJson<User>("Admin");

            // Kiểm tra nếu user null hoặc không có id
            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("Login", "Home", new { area = "Admin" });
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

        [HttpGet]
        [Route("Admin/Order/UpdateStatus")]
        public IActionResult UpdateStatus(string orderStatus, string orderId)
        {
            // Tìm đơn hàng theo OrderId
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

            // Nếu đơn hàng không tồn tại, trả về lỗi
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = orderStatus;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Redirect về trang chi tiết đơn hàng hoặc trang danh sách đơn hàng
            return RedirectToAction("Index", "Order");
        }

    }
}
