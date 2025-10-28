using GameVISION.Core.DTOs;
using GameVISION.Core.Entities;
using GameVISION.Core.Helpers;
using GameVISION.Infrastracture.AuthRepo;

namespace GameVISION.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthService(IAuthRepository authRepository)
        {
            _repo = authRepository;
        }
        public async Task<ResponseModel<User?>> Login(LoginDTO dTO)
        {
            var response = new ResponseModel<User?>() { IsSuccess = false,Message = "user not found please sign up",TotalCount = null , Result = null };
            try
            {
                var user  = await _repo.Login(dTO);
                if(user != null)
                {
                    response.IsSuccess = true;
                    response.Message = "";
                    response.Result = user;
                    return response;
                }
                return response;
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;  
                response.IsSuccess = false;
                return response;
            }
        }

        public async Task<ResponseModel> Register(RegisterDTO dTO)
        {
            var response = new ResponseModel() { Message = "email or username is Exist right now try another names" , IsSuccess = false};
            try
            {
                var res = await _repo.RegisterAsync(dTO);
                if (res)
                {
                    response.IsSuccess = true;
                    response.Message = "sign up success";
                    return response;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
