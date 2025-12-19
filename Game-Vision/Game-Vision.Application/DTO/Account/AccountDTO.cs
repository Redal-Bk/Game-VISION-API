namespace Game_Vision.Application.DTO.Account
{
    // RegisterDto.cs
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }

    // LoginDto.cs
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    // AuthResponseDto.cs
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
