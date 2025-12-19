using Game_Vision.Application.DTO.Account;
using Game_Vision.Application.Interface;
using Game_Vision.Domain;
using Game_Vision.Models;
using MediatR;


namespace Game_Vision.Application.Command.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly GameVisionDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterCommandHandler(GameVisionDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = 3,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = "User",
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
}
