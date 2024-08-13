using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater.Areas.Booking.Services;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class ECPController : Controller
    {
        private readonly Theater_ProjectDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ECPController(UserManager<ApplicationUser> userManager, Theater_ProjectDbContext context, IHttpContextAccessor accessor)
        {
            _context = context; _accessor = accessor;
            _userManager = userManager;

        }
        public IActionResult Index(int totalAmount, string merchantTradeNo)
        {
            //還要帶入user的Email
            // If merchantTradeNo is not provided, generate a new one
            if (string.IsNullOrEmpty(merchantTradeNo))
            {
                merchantTradeNo = GenerateUniqueTradeNo();
            }

            // Create a view model to pass data to the view
            var viewModel = new ECPayViewModel
            {
                TotalAmount = totalAmount,
                MerchantTradeNo = merchantTradeNo
            };

            return View(viewModel);
        }
        private string GenerateUniqueTradeNo()
        {
            // Implement your logic to generate a unique trade number
            return "TR" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        }


        public IActionResult GetPaymentForm(int totalPrice, string merchantTradeNo)
        {
            var model = new ECPayViewModel
            {
                TotalAmount = totalPrice,
                MerchantTradeNo = merchantTradeNo,
                ExpireDate = DateTime.Now.AddDays(3).ToString("yyyy/MM/dd")
            };
            return PartialView("~/Areas/Booking/Views/PartialView/_ECPayForm.cshtml", model);
        }
        public IActionResult SendToNewebPay(ECPayViewModel inModel)
        {
            var userId = GetCurrentUserId();

            //var service = GetPayType(inModel.PayOption);
            var service = new ECPayService(_context, _accessor, userId);

            return Json(GetReturnValue(service, inModel));
        }
        private string GetReturnValue(ICommerce service, ECPayViewModel inModel)
        {
            switch (inModel.PayOption)
            {
                case "newbPay":
                    return service.GetCallBack(inModel);
                case "newbPayPeriod":
                    return service.GetPeriodCallBack(inModel);
                case "ECPay":
                    return service.GetCallBack(inModel);
                case "ECPayPeriod":
                    return service.GetPeriodCallBack(inModel);

                default: throw new ArgumentException("No Such option");
            }
        }
        public IActionResult CallbackReturn(string option)
        {
            var userId = GetCurrentUserId();

            var service = new ECPayService(_context, _accessor, userId);
            var result = service.GetCallbackResult(Request.Form);
            ViewData["ReceiveObj"] = result.ReceiveObj;
            ViewData["TradeInfo"] = result.TradeInfo;
            ViewData["MerchantTradeNo"] = result.MerchantTradeNo;
            ViewData["RtnMsg"] = result.RtnMsg;
            return View();
        }
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

}
