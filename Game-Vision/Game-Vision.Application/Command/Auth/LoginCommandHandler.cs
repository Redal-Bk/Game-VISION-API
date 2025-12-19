using Game_Vision.Application.DTO.Account;
using Game_Vision.Application.Interface;
using Game_Vision.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Game_Vision.Application.Command.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly GameVisionDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(GameVisionDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("نام کاربری یا رمز عبور اشتباه است");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("حساب شما غیرفعال است");

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.Name,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
}
