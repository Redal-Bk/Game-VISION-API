using GameVISION.Core.DTOs;
using GameVISION.Core.Entities;
using GameVISION.Core.Helpers;
using GameVISION.Infrastracture.MyDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static GameVISION.Infrastracture.AuthRepo.IAuthRepository;


namespace GameVISION.Infrastracture.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly GameVisionDbContext _contex;
        private readonly JwtSettings _jwtSettings;
        public AuthRepository(GameVisionDbContext gameVisionDbContext, JwtSettings jwtSettings)
        {
            _contex = gameVisionDbContext;
            _jwtSettings = jwtSettings;
        }
        public async Task<User?> Login(LoginDTO dTO)
        {
            try
            {
                var user = await _contex.Users.FirstOrDefaultAsync(x => x.Email == dTO.Email && x.PasswordHash == dTO.Password);
                if (user == null)
                {
                    return null;
                }
                return user;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDTO dTO)
        {
            try
            {
                var user = await _contex.Users.FirstOrDefaultAsync(x => x.Email == dTO.Email || x.Username == dTO.Fullname);
                if (user == null)
                {
                    var User = new User
                    {
                        Username = dTO.Fullname,
                        Email = dTO.Email,
                        PasswordHash = dTO.PasswordHash,

                    };
                    await _contex.Users.AddAsync(User);
                    await _contex.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private AuthResult GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.UserId.ToString())
            // هر claim دلخواه دیگر، مثل roles
        };

            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResult(tokenString, expires);
        }
    }
}
