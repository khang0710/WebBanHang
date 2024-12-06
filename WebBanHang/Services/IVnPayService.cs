using WebBanHang.Models.VNPay;

namespace WebBanHang.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string orderID);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
