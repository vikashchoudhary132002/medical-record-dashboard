using MedicalDashboardAPI.Data;
using MedicalDashboardAPI.DTOs;
using MedicalDashboardAPI.Models; // Add this line
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FilesController> _logger;
        private const string UploadsFolder = "Uploads";
        //private readonly string UploadsFolder = "uploads";  // ✅ CORRECT PLACE
        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
        private static readonly string[] AllowedFileTypes = {
            "Lab Report", "Prescription", "X-Ray", "Blood Report", "MRI Scan", "CT Scan"
        };

        public FilesController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<FilesController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

      
        [HttpPost("upload")]
        public async Task<ActionResult<FileResponseDto>> UploadFile([FromForm] FileUploadDto fileUpload)
        {
            try
            {
                // Check session
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                // Validate file type (from document's dropdown options)
                if (!AllowedFileTypes.Contains(fileUpload.FileType))
                    return BadRequest("Invalid file type");

                // Validate file extension
                var fileExtension = Path.GetExtension(fileUpload.File.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(fileExtension))
                    return BadRequest($"Invalid file extension. Allowed: {string.Join(", ", AllowedExtensions)}");

                // Validate file size (10MB max)
                if (fileUpload.File.Length > 10 * 1024 * 1024)
                    return BadRequest("File size exceeds 10MB limit");

                // ========== MODIFIED SECTION ========== //
                // Create uploads directory using content root path or custom path
                var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads");
                // Alternative: Use a system-wide location
                // var uploadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YourAppName", "Uploads");

                Directory.CreateDirectory(uploadsPath);
                // ========== END MODIFIED SECTION ========== //

                // Save file
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                //var filePath = Path.Combine(uploadsPath, uniqueFileName);
                //var filePath = Path.Combine("uploads", uniqueFileName); // or wherever you are saving file
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await fileUpload.File.CopyToAsync(stream);
                //var filePath = Path.Combine("uploads", uniqueFileName); // or wherever you are saving file

                var medicalFile = new MedicalFile
                {
                    FileType = fileUpload.FileType,
                    FileName = fileUpload.FileName,
                    StoredFileName = uniqueFileName,
                    ContentType = fileUpload.File.ContentType,
                    FileSize = fileUpload.File.Length,
                    UserId = userId.Value,
                    FilePath = filePath  // ✅ fixed
                };


                _context.MedicalFiles.Add(medicalFile);
                await _context.SaveChangesAsync();

                return Ok(new FileResponseDto
                {
                    Id = medicalFile.Id,
                    FileName = medicalFile.FileName,
                    FileType = medicalFile.FileType,
                    UploadDate = medicalFile.UploadDate,
                    FileSize = medicalFile.FileSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "File upload failed");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<FileDto>>> GetUserFiles()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var files = await _context.MedicalFiles
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.UploadDate)
                    .Select(f => new FileDto
                    {
                        Id = f.Id,
                        FileName = f.FileName,
                        FileType = f.FileType,
                        UploadDate = f.UploadDate,
                        FileSize = f.FileSize,
                        FileUrl = $"/uploads/{f.StoredFileName}" // For frontend access
                    })
                    .ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving files");
                return StatusCode(500, "Failed to load files");
            }
        }

        
        //private readonly string UploadsFolder = "uploads";

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var file = await _context.MedicalFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

                if (file == null) return NotFound();

                var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", file.StoredFileName);
                _logger.LogInformation($"Download path: {filePath}");

                if (!System.IO.File.Exists(filePath)) return NotFound("File not found on server.");

                return PhysicalFile(filePath, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file ID: {id}");
                return StatusCode(500, "Download failed due to server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var file = await _context.MedicalFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

                if (file == null) return NotFound();

                // Delete physical file using ContentRootPath
                var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", file.StoredFileName);
                _logger.LogInformation($"Deleting file from path: {filePath}");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully.");
                }
                else
                {
                    _logger.LogWarning("File not found on disk during deletion.");
                }

                // Delete DB record
                _context.MedicalFiles.Remove(file);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file ID: {id}");
                return StatusCode(500, "Deletion failed due to server error.");
            }
        }
    }

    public class FileResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public long FileSize { get; set; }
    }
}