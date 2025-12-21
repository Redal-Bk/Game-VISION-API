using Game_Vision.Application.DTO.UserPcReq;
using Game_Vision.Domain;
using Game_Vision.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Game_Vision.Application.Command.UserPsReq
{
    public class UpdateUserPCSpecsHandler : IRequestHandler<UpdateUserPCSpecsCommand, UserPCSpecsDto>
    {
        private readonly GameVisionDbContext _context;

        public UpdateUserPCSpecsHandler(GameVisionDbContext context)
        {
            _context = context;
        }

        public async Task<UserPCSpecsDto> Handle(UpdateUserPCSpecsCommand request, CancellationToken ct)
        {
            var specs = await 
                 _context
                .UserPcspecs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, ct);

            if (specs == null)
            {
                specs = new UserPcspec
                {
                    UserId = request.UserId,
                    Os = request.OS,
                    Cpu = request.CPU,
                    Ram = request.RAM,
                    Gpu = request.GPU,
                    DirectX = request.DirectX,
                    StorageAvailable = request.Storage,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserPcspecs.Add(specs);
            }
            else
            {
                specs.Os = request.OS;
                specs.Cpu = request.CPU;
                specs.Ram = request.RAM;
                specs.Gpu = request.GPU;
                specs.DirectX = request.DirectX;
                specs.StorageAvailable = request.Storage;
                specs.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);

            return new UserPCSpecsDto
            {
                // مپ به DTO
                OS = specs.Os,
                CPU = specs.Cpu,
                RAM = specs.Ram ?? 1,
                GPU = specs.Gpu,
                DirectX = specs.DirectX,
                Storage = specs.StorageAvailable ?? 0,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
