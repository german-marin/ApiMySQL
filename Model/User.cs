using System.ComponentModel.DataAnnotations;
namespace ApiMySQL.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int Active { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
