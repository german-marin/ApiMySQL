using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAndPassword(string username, string password);
        string GenerateJwtToken(User user);
    }
}
