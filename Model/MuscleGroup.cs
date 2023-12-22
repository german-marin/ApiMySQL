using System.ComponentModel.DataAnnotations;

namespace ApiMySQL.Model
{
    public class MuscleGroup
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        public string ImageFront { get; set; }
        public string ImageRear { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
