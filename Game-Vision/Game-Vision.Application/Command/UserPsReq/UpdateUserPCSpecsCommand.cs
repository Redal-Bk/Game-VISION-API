using Game_Vision.Application.DTO.UserPcReq;
using MediatR;

namespace Game_Vision.Application.Command.UserPsReq
{
    public class UpdateUserPCSpecsCommand : IRequest<UserPCSpecsDto>
    {
        public int UserId { get; set; }
        public string OS { get; set; } = null!;
        public string CPU { get; set; } = null!;
        public int RAM { get; set; }
        public string GPU { get; set; } = null!;
        public string DirectX { get; set; } = null!;
        public int Storage { get; set; }
    }
}
