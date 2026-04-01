using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;

    public UploadsController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<ApiResponse<UploadImageResponse>>> UploadImage([FromForm] UploadImageRequest request)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<UploadImageResponse>.Fail("Please select an image file."));
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(ApiResponse<UploadImageResponse>.Fail("Image size cannot exceed 5MB."));
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse<UploadImageResponse>.Fail("Only jpg/jpeg/png/webp/gif are supported."));
        }

        var wwwroot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(wwwroot))
        {
            wwwroot = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var uploadDir = Path.Combine(wwwroot, "uploads", "products");
        Directory.CreateDirectory(uploadDir);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var savePath = Path.Combine(uploadDir, fileName);

        await using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";
        return Ok(ApiResponse<UploadImageResponse>.Ok(new UploadImageResponse { Url = imageUrl }, "uploaded"));
    }
}

public class UploadImageResponse
{
    public string Url { get; set; } = string.Empty;
}

public class UploadImageRequest
{
    public IFormFile? File { get; set; }
}

