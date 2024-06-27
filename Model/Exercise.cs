using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMySQL.Model
{
    public class Exercise
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; } = true; // Valor por defecto: true
        public DateTime LastUpdated { get; set; }

        //[ForeignKey("CategoryID")]
        //public Category Category { get; set; }
    }
}
