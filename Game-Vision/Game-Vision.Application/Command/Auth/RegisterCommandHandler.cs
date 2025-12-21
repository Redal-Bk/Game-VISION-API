using Game_Vision.Application.DTO.Account;
using Game_Vision.Application.Interface;
using Game_Vision.Domain;
using Game_Vision.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Game_Vision.Application.Command.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly GameVisionDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterCommandHandler(
            GameVisionDbContext context,
            IJwtTokenGenerator jwtTokenGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);
            if (exists)
                throw new InvalidOperationException("نام کاربری یا ایمیل قبلاً استفاده شده است");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = 3, // User عادی
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(user).Reference(u => u.Role).LoadAsync(cancellationToken);

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
                Token = token, // برای JS اگر بخواد
                Username = user.Username,
                Role = user.Role?.Name ?? "User",
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
}