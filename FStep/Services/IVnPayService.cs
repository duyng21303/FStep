using FStep.ViewModels;

namespace FStep.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequestModel model);
        VnPayResponseModel PaymentExecute(IQueryCollection collection);
    }
}
