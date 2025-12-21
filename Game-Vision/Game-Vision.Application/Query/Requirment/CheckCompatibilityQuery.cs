using Game_Vision.Application.DTO.CompatibilityResultDto;
using MediatR;

namespace Game_Vision.Application.Query.Requirment
{
    public class CheckCompatibilityQuery : IRequest<CompatibilityResultDto>
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
    }
}
