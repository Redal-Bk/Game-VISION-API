using GameVISION.Application.Auth;
using GameVISION.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameVISION.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dTO)
        {
            var res = await _authService.Login(dTO);    
            if(res.IsSuccess)
                return Ok(res);
            return BadRequest(res);
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> Register(RegisterDTO dTO)
        {
            var res  = await _authService.Register(dTO);
            if(res.IsSuccess)
                return Ok(res);
            return BadRequest(res);
        }
    }
}
