using Game_Vision.Application.DTO.Account;
using MediatR;


namespace Game_Vision.Application.Command.Auth
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
