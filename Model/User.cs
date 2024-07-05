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

        //[Key]
        //public int Id { get; set; }
        //[Required]
        //public string Username { get; set; }
        //[Required]
        //public string Password { get; set; }
        //public string? Role { get; set; }
        //[Required]
        //public int Active { get; set; }
        //public DateTime? LastUpdated { get; set; }
        //[Required]
        public string SchemaName { get; set; } // Campo para almacenar el esquema de base de datos del cliente
    }
}
