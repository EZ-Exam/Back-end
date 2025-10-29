using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Supabase;
using Supabase.Storage;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/document-blob")]
    [Produces("application/json")]
    [Authorize]
    public class DocumentBlobController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<DocumentBlobController> _logger;
        private readonly IConfiguration _configuration;

        public DocumentBlobController(Supabase.Client supabaseClient, IConfiguration configuration, ILogger<DocumentBlobController> logger)
        {
            _supabaseClient = supabaseClient;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Upload a document file (PDF or Word) to Supabase storage
        /// </summary>
        /// <param name="file">Document file to upload (PDF or Word)</param>
        /// <param name="folder">Optional folder path (default: "uploads")</param>
        /// <returns>Upload result with file URL</returns>
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Upload document file", Description = "Uploads a PDF or Word document to Supabase storage")]
        [SwaggerResponse(200, "File uploaded successfully", typeof(DocumentUploadResponse))]
        [SwaggerResponse(400, "Invalid file or request", typeof(object))]
        [SwaggerResponse(500, "Upload failed", typeof(object))]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromQuery] string folder = "uploads")
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { Message = "No file provided." });
                }

                // Check file type (PDF or Word)
                var allowedTypes = new[] { 
                    "application/pdf", 
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
                    "application/msword" // .doc
                };
                
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { Message = "Only PDF and Word documents are allowed." });
                }

                // Check file size (max 50MB)
                if (file.Length > 50 * 1024 * 1024)
                {
                    return BadRequest(new { Message = "File size cannot exceed 50MB." });
                }

                // Get user ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = $"{folder}/{userId}/{fileName}";

                // Initialize Supabase storage
                await _supabaseClient.InitializeAsync();
                var storage = _supabaseClient.Storage;
                var bucketName = _configuration["Supabase:Storage:BucketName"] ?? "pdf-documents";

                // Bucket should be created manually in Supabase Dashboard

                // Upload file
                using var stream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var result = await storage
                    .From(bucketName)
                    .Upload(fileBytes, filePath, new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        Upsert = false
                    });

                if (result == null)
                {
                    return StatusCode(500, new { Message = "Failed to upload file to storage." });
                }

                // Get public URL
                var publicUrl = storage
                    .From(bucketName)
                    .GetPublicUrl(filePath);

                _logger.LogInformation("Document uploaded successfully: {FilePath} by user {UserId}", filePath, userId);

                var fileType = fileExtension.ToLower() switch
                {
                    ".pdf" => "PDF",
                    ".docx" => "Word Document",
                    ".doc" => "Word Document",
                    _ => "Unknown"
                };

                var response = new DocumentUploadResponse
                {
                    FileName = fileName,
                    FilePath = filePath,
                    PublicUrl = publicUrl,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UploadedAt = DateTime.UtcNow,
                    UserId = userId,
                    FileType = fileType
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document file");
                return StatusCode(500, new { Message = "An error occurred while uploading the file." });
            }
        }

        /// <summary>
        /// Download a document file from Supabase storage
        /// </summary>
        /// <param name="filePath">Path to the file in storage</param>
        /// <returns>Document file content</returns>
        [HttpGet("download")]
        [SwaggerOperation(Summary = "Download document file", Description = "Downloads a PDF or Word document from Supabase storage")]
        [SwaggerResponse(200, "File downloaded successfully")]
        [SwaggerResponse(404, "File not found", typeof(object))]
        [SwaggerResponse(500, "Download failed", typeof(object))]
        public async Task<IActionResult> DownloadDocument([FromQuery] [Required] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { Message = "File path is required." });
                }

                // Initialize Supabase storage
                await _supabaseClient.InitializeAsync();
                var storage = _supabaseClient.Storage;
                var bucketName = _configuration["Supabase:Storage:BucketName"] ?? "pdf-documents";

                // Download file
                var fileBytes = await storage
                    .From(bucketName)
                    .Download(filePath, null);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return NotFound(new { Message = "File not found." });
                }

                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();
                
                // Determine content type based on file extension
                string contentType = fileExtension switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".doc" => "application/msword",
                    _ => "application/octet-stream"
                };
                
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document file: {FilePath}", filePath);
                return StatusCode(500, new { Message = "An error occurred while downloading the file." });
            }
        }

        /// <summary>
        /// Get public URL for a document file
        /// </summary>
        /// <param name="filePath">Path to the file in storage</param>
        /// <returns>Public URL for the file</returns>
        [HttpGet("url")]
        [SwaggerOperation(Summary = "Get document file URL", Description = "Gets the public URL for a PDF or Word document")]
        [SwaggerResponse(200, "URL retrieved successfully", typeof(DocumentUrlResponse))]
        [SwaggerResponse(400, "Invalid file path", typeof(object))]
        public IActionResult GetDocumentUrl([FromQuery] [Required] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { Message = "File path is required." });
                }

                var storage = _supabaseClient.Storage;
                var bucketName = _configuration["Supabase:Storage:BucketName"] ?? "pdf-documents";

                var publicUrl = storage
                    .From(bucketName)
                    .GetPublicUrl(filePath);

                var response = new DocumentUrlResponse
                {
                    FilePath = filePath,
                    PublicUrl = publicUrl
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document URL: {FilePath}", filePath);
                return StatusCode(500, new { Message = "An error occurred while getting the file URL." });
            }
        }

        /// <summary>
        /// Delete a document file from Supabase storage
        /// </summary>
        /// <param name="filePath">Path to the file in storage</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("delete")]
        [SwaggerOperation(Summary = "Delete document file", Description = "Deletes a PDF or Word document from Supabase storage")]
        [SwaggerResponse(200, "File deleted successfully", typeof(object))]
        [SwaggerResponse(404, "File not found", typeof(object))]
        [SwaggerResponse(500, "Deletion failed", typeof(object))]
        public async Task<IActionResult> DeleteDocument([FromQuery] [Required] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { Message = "File path is required." });
                }

                // Get user ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                // Initialize Supabase storage
                await _supabaseClient.InitializeAsync();
                var storage = _supabaseClient.Storage;
                var bucketName = _configuration["Supabase:Storage:BucketName"] ?? "pdf-documents";

                // Delete file
                var result = await storage
                    .From(bucketName)
                    .Remove(filePath);

                if (result == null)
                {
                    return NotFound(new { Message = "File not found or could not be deleted." });
                }

                _logger.LogInformation("Document deleted successfully: {FilePath} by user {UserId}", filePath, userId);

                return Ok(new { Message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document file: {FilePath}", filePath);
                return StatusCode(500, new { Message = "An error occurred while deleting the file." });
            }
        }

        /// <summary>
        /// List document files in a folder
        /// </summary>
        /// <param name="folder">Folder path to list files from</param>
        /// <returns>List of document files</returns>
        [HttpGet("list")]
        [SwaggerOperation(Summary = "List document files", Description = "Lists PDF and Word document files in a specified folder")]
        [SwaggerResponse(200, "Files listed successfully", typeof(DocumentListResponse))]
        [SwaggerResponse(500, "List operation failed", typeof(object))]
        public async Task<IActionResult> ListDocuments([FromQuery] string folder = "uploads")
        {
            try
            {
                // Get user ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                // Initialize Supabase storage
                await _supabaseClient.InitializeAsync();
                var storage = _supabaseClient.Storage;
                var bucketName = _configuration["Supabase:Storage:BucketName"] ?? "pdf-documents";

                // List files in user's folder
                var userFolder = $"{folder}/{userId}";
                var files = await storage
                    .From(bucketName)
                    .List(userFolder);

                var documentFiles = files?.Where(f => 
                    f.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                    f.Name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ||
                    f.Name.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
                    .Select(f => 
                    {
                        long fileSize = 0;
                        if (f.MetaData != null && f.MetaData.TryGetValue("size", out var sizeObj))
                        {
                            if (sizeObj is long size)
                                fileSize = size;
                            else if (sizeObj is int intSize)
                                fileSize = intSize;
                        }

                        var fileExtension = Path.GetExtension(f.Name).ToLower();
                        var fileType = fileExtension switch
                        {
                            ".pdf" => "PDF",
                            ".docx" => "Word Document",
                            ".doc" => "Word Document",
                            _ => "Unknown"
                        };

                        return new DocumentFileInfo
                        {
                            Name = f.Name,
                            Path = $"{userFolder}/{f.Name}",
                            Size = fileSize,
                            LastModified = f.UpdatedAt ?? DateTime.MinValue,
                            PublicUrl = storage.From(bucketName).GetPublicUrl($"{userFolder}/{f.Name}"),
                            FileType = fileType,
                            FileExtension = fileExtension
                        };
                    })
                    .ToList() ?? new List<DocumentFileInfo>();

                var response = new DocumentListResponse
                {
                    Folder = userFolder,
                    Files = documentFiles,
                    TotalCount = documentFiles.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing document files in folder: {Folder}", folder);
                return StatusCode(500, new { Message = "An error occurred while listing files." });
            }
        }
    }

    // Response models
    public class DocumentUploadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }

    public class DocumentUrlResponse
    {
        public string FilePath { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
    }

    public class DocumentListResponse
    {
        public string Folder { get; set; } = string.Empty;
        public List<DocumentFileInfo> Files { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class DocumentFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string PublicUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
    }
}
