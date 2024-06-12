namespace ApiMySQL.Model
{
    public interface IJwtSettings
    {
        string SecretKey { get; }
        string Issuer { get; }
        string Audience { get; }
        string Subject { get; }
    }
}
