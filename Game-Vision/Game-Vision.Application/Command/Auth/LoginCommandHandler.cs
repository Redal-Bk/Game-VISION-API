using Game_Vision.Application.DTO.Account;
using Game_Vision.Application.Interface;
using Game_Vision.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Game_Vision.Application.Command.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly GameVisionDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginCommandHandler(
            GameVisionDbContext context,
            IJwtTokenGenerator jwtTokenGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
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

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenGenerator.GenerateToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(24)
            };

            _httpContextAccessor.HttpContext!.Response.Cookies.Append("authToken", token, cookieOptions);

            return new AuthResponseDto
            {
                Token = token, 
                Username = user.Username,
                Role = user.Role?.Name ?? "User",
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
}
