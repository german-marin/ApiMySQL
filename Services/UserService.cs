using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ApiMySQL.Services
{
    public class UserService : IUserService
    {
        private readonly CommonDbContext _context;
        private readonly IJwtSettings _jwtSettings;

        public UserService(CommonDbContext context, IJwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    Log.Logger.Information("User {Username} successfully authenticated.", username);
                    return user;
                }
                else
                {
                    Log.Logger.Warning("Failed authentication attempt for username {Username}.", username);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving user by username {Username}", username);
                throw;
            }
        }

        public string GenerateJwtToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim("SchemaName", user.SchemaName),
                        new Claim(JwtRegisteredClaimNames.Sub, _jwtSettings.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                Log.Logger.Information("JWT token generated successfully for user ID {UserId}.", user.Id);

                return tokenString;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error generating JWT token for user ID {UserId}.", user.Id);
                throw;
            }
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                Log.Logger.Information("JWT token validated successfully.");

                return principal;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error validating JWT token.");
                return null;
            }
        }
    }
}
