using Game_Vision.Application.Command.Auth;
using MediatR;
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
    }
}