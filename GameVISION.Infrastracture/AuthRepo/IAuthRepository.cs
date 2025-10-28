using GameVISION.Core.DTOs;
using GameVISION.Core.Entities;


namespace GameVISION.Infrastracture.AuthRepo
{
    public interface IAuthRepository
    {
        Task<bool> RegisterAsync(RegisterDTO dTO);
        Task<User?> Login(LoginDTO dTO);
    }
}
