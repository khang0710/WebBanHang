using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.ViewModels;
using WebBanHang.Models.VNPay;
using WebBanHang.Repository;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        SnackStoreContext db = new SnackStoreContext();

        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel pay, CartItemViewModel model)
        {
            Order order = model.NewOrder;
            order.Payment = "VNPAY - Chưa thanh toán";

            db.Orders.Add(order);
            db.SaveChanges();

            string newOrderId = order.OrderId;
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

            pay.CreatedDate = DateTime.Now;
            return Redirect(_vnPayService.CreatePaymentUrl(pay, HttpContext, newOrderId));
        }


        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {
                string orderID = response.OrderDescription;
                var order = db.Orders.FirstOrDefault(o => o.OrderId == orderID);
                order.Payment = "VNPay - Đã thanh toán";
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToAction("Profile", "Account");
            }
            return Json(response);
        }

    }
}
