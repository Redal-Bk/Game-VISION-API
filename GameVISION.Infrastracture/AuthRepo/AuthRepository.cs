using GameVISION.Core.DTOs;
using GameVISION.Core.Entities;
using GameVISION.Infrastracture.MyDbContext;
using Microsoft.EntityFrameworkCore;

namespace GameVISION.Infrastracture.AuthRepo
{
    public class AuthRepository : IAuthRepository
    {
        private readonly GameVisionDbContext _contex;

        public AuthRepository(GameVisionDbContext gameVisionDbContext)
        {
            _contex = gameVisionDbContext;
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
    }
}
