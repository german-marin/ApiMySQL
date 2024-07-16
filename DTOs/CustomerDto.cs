namespace ApiMySQL.DTOs
{
    public class CustomerDto
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public int? PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? UnregistrationDate { get; set; }
        public string Notes { get; set; }
    }
}
