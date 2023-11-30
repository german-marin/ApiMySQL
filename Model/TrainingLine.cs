namespace ApiMySQL.Model
{
    public class TrainingLine
    {
        public int ID { get; set; }
        public int IdExercise { get; set; }
        public int IdTraining { get; set; }
        public string Series { get; set; }
        public string Repetition { get; set; }
        public string Weight { get; set; }
        public string Recovery { get; set; }
        public string Others { get; set; }
        public string Notes { get; set; }
    }
}
