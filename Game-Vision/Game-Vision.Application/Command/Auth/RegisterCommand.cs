using Game_Vision.Application.DTO.Account;
using MediatR;

namespace Game_Vision.Application.Command.Auth
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
