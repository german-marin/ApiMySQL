using ApiMySQL.Model;
using System.Security.Claims;

namespace ApiMySQL.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAndPassword(string username, string password);
        string GenerateJwtToken(User user);
        ClaimsPrincipal ValidateJwtToken(string token);
    }
}
