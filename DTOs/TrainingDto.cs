namespace ApiMySQL.DTOs
{
    public class TrainingDto
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerID { get; set; }
        public string Notes { get; set; }
        public string Days { get; set; }
    }
}
