namespace ApiMySQL.Model
{
    public class Training
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IdClient { get; set; }
        public string Notes { get; set; }
    }
}
