using MySqlX.XDevAPI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMySQL.Model
{
    public class Training
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerID { get; set; }
        public string Notes { get; set; }
        public DateTime LastUpdate { get; set; }

        //[ForeignKey("CustomerID")]
        //public Customer Customer { get; set; }
    }
}
