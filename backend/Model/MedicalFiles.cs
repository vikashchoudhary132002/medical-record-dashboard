namespace MedicalDashboardAPI.Models
{
    public class MedicalFile
    {
        public int Id { get; set; }
        public required string FileName { get; set; }

        public required string FilePath { get; set; }
        public required string FileType { get; set; }
        public required string StoredFileName { get; set; }
        public required string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User? User { get; set; } // Made nullable
    }
}