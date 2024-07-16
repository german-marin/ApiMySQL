using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiMySQL.Model
{
    public class Category
    { 
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        [Required]
        public int MuscleGroupID { get; set; }
        public DateTime? LastUpdate { get; set; }

        //[ForeignKey("MuscleGroupID")]
        //public MuscleGroup MuscleGroup { get; set; }

        //// Propiedad de navegación
        //public ICollection<Exercise> Exercises { get; set; }
    }
}
