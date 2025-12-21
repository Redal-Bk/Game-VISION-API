using Game_Vision.Application.DTO.UserPcReq;
using Game_Vision.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Game_Vision.Application.Query.GetUserSpec
{
    public class GetUserPCSpecsHandler : IRequestHandler<GetUserPCSpecsQuery, UserPCSpecsDto>
    {
        private readonly GameVisionDbContext _context;

        public GetUserPCSpecsHandler(GameVisionDbContext context)
        {
            _context = context;
        }

        public async Task<UserPCSpecsDto> Handle(GetUserPCSpecsQuery request, CancellationToken ct)
        {
            var specs = await _context.UserPcspecs
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, ct);

            if (specs == null)
            {

                return new UserPCSpecsDto
                {
                    UserId = request.UserId,
                    OS = "",
                    CPU = "",
                    RAM = 8,
                    GPU = "",
                    DirectX = "",
                    Storage = 256
                };
            }

            return new UserPCSpecsDto
            {
                Id = specs.Id,
                UserId = specs.UserId,
                OS = specs.Os ?? "",
                CPU = specs.Cpu ?? "",
                RAM = specs.Ram ?? 0,
                GPU = specs.Gpu ?? "",
                DirectX = specs.DirectX ?? "",
                Storage = specs.StorageAvailable ?? 0,
                CreatedAt = specs.CreatedAt,
                UpdatedAt = specs.UpdatedAt
            };
        }
    }
}
