namespace MedicalDashboardAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public List<MedicalFile> MedicalFiles { get; set; } = new(); // Initialize empty list
    }
}