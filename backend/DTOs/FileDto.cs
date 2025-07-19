namespace MedicalDashboardAPI.DTOs
{
    public class FileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public long FileSize { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }
}