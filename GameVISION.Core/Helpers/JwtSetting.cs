namespace GameVISION.Core.Helpers
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
    }

}
