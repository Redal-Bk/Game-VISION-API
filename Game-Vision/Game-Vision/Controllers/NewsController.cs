using Microsoft.AspNetCore.Mvc;

namespace Game_Vision.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NewsDetail()
        {
            return View();
        }
    }
}
