using Game_Vision.Application.Command.UserPsReq;
using Game_Vision.Application.DTO.UpdateUserOcSpec;
using Game_Vision.Application.Query.GetUserSpec;
using Game_Vision.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Game_Vision.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IMediator _mediator;
        private readonly GameVisionDbContext _context;
        public ProfileController(IMediator mediator, GameVisionDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            ViewData["Cpus"] = await _context.CpuList.ToListAsync();
            ViewData["Gpus"] = await _context.GpuList.ToListAsync();
            ViewData["Oses"] = await _context.OsList.ToListAsync();
            ViewData["Rams"] = await _context.RamList.ToListAsync();
            ViewData["Storages"] = await _context.StorageList.ToListAsync();
            var query = new GetUserPCSpecsQuery { UserId = userId };
            var model = await _mediator.Send(query);

            return View(model);

        }

      
        [HttpPost("pcspecs")]
        public async Task<IActionResult> EditPCSpecs([FromForm]UpdateUserPCSpecsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var command = new UpdateUserPCSpecsCommand
            {
                UserId = userId,
                OS = model.OS,
                CPU = model.CPU,
                RAM = model.RAM,
                GPU = model.GPU,
                DirectX = model.DirectX,
                Storage = model.Storage
            };

            await _mediator.Send(command);

            TempData["Success"] = "مشخصات سیستم با موفقیت ذخیره شد!";
            return RedirectToAction("Index");
        }
    }
}
