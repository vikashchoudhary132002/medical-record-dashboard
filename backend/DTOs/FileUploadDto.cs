using System.ComponentModel.DataAnnotations;

public class FileUploadDto
{
    [Required]
    public required string FileType { get; set; }

    [Required]
    [StringLength(100)]
    public required string FileName { get; set; }

    [Required]
    public required IFormFile File { get; set; }
}