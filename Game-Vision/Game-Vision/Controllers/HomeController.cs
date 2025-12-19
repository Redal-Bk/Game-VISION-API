using Microsoft.AspNetCore.Mvc;

namespace Game_Vision.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

      
    }
}
