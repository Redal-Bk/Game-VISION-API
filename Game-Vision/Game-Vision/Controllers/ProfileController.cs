using Microsoft.AspNetCore.Mvc;

namespace Game_Vision.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
