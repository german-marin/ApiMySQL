namespace ApiMySQL.Model
{
    public class JwtSettings : IJwtSettings
    {
        public string SecretKey { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
