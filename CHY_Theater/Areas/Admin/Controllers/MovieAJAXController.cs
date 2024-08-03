using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Admin.Controllers
{
    public class MovieAJAXController : Controller
    {
        [Area("Admin")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
