namespace GameVISION.Core.DTOs
{
    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Fullname { get;set;  } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
