using Microsoft.AspNetCore.Mvc;

namespace Game_Vision.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GameDetail()
        {
            return View();
        }
    }
}
