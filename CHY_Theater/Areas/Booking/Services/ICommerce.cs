using CHY_Theater.Areas.Booking.Models.ViewModels;

namespace CHY_Theater.Areas.Booking.Services
{
    public interface ICommerce
    {
        string GetCallBack(ECPayViewModel inModel);
        string GetPeriodCallBack(ECPayViewModel inModel);
        Result GetCallbackResult(IFormCollection form);
    }
}
