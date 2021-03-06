namespace MyUniversity.UserManager.Services.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpirationDays { get; set; }
    }
}
