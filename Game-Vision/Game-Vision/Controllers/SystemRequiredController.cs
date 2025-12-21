using Game_Vision.Application.DTO.CompatibilityResultDto;
using Game_Vision.Application.Query.Requirment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Game_Vision.Controllers
{
    public class SystemRequiredController : Controller
    {
        private readonly IMediator _mediator;

        public SystemRequiredController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult<CompatibilityResultDto>> CheckCompatibility(int gameId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var query = new CheckCompatibilityQuery
            {
                GameId = gameId,
                UserId = userId
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
