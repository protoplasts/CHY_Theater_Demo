using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
