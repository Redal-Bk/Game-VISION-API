using Game_Vision.Application.Command.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Game_Vision.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Discord(string returnUri = "https://localhost:7214/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUri }, "Discord");
        }
        [HttpGet]
        public IActionResult CallBack(string code, string state)
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();

            // برای نمایش ساده توی View
            return View(claims);
        }
        // ثبت‌نام
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Register([FromBody]RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(); 
            }

            try
            {
                var result = await _mediator.Send(command);
     
                return Ok(result);
            }
            catch (Exception ex)
            {
               
                return BadRequest();
            }
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            // کوکی Authentication رو پاک می‌کنه و کاربر رو Sign Out می‌کنه
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

           
            return Redirect("/");
        }
    }
}