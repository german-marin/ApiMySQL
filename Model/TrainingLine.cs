using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiMySQL.Model
{
    public class TrainingLine
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ExerciseID { get; set; }
        [Required]
        public int TrainingID { get; set; }
        public string Sets { get; set; }
        public string Repetitions { get; set; }
        public string Weight { get; set; }
        public string Recovery { get; set; }
        public string Others { get; set; }
        public string Notes { get; set; }
        public string? Grip {  get; set; }
        public DateTime? LastUpdated { get; set; }

        //[ForeignKey("ExerciseID")]
        //public Exercise Exercise { get; set; }

        //[ForeignKey("TrainingID")]
        //public Training Training { get; set; }
   
    }
}
