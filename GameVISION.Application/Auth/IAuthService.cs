using GameVISION.Core.DTOs;
using GameVISION.Core.Entities;
using GameVISION.Core.Helpers;

namespace GameVISION.Application.Auth
{
    public interface IAuthService
    {
        Task<ResponseModel<User?>> Login(LoginDTO dTO);
        Task<ResponseModel> Register(RegisterDTO dTO);
    }
}
