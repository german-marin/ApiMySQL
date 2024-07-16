namespace ApiMySQL.DTOs
{
    public class TrainingLineDto
    {
        public int ID { get; set; }
        public int ExerciseID { get; set; }
        public int TrainingID { get; set; }
        public string Sets { get; set; }
        public string Repetitions { get; set; }
        public string Weight { get; set; }
        public string Recovery { get; set; }
        public string Others { get; set; }
        public string Notes { get; set; }
        public string? Grip { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
