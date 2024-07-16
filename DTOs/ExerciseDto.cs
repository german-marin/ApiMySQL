namespace ApiMySQL.DTOs
{
    public class ExerciseDto
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; } = true;
    }

}
