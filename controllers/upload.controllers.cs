using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;

namespace Controllers
{
    [ApiController]
    [Route("api/images")]
    public class NameController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _accessor;

        public NameController(AppDbContext db, IHttpContextAccessor accessor)
        {
            _db = db;
            _accessor = accessor;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult(new { message = " invalid image format" });
                }

                var user = _accessor?.HttpContext?.User;
                var userId = User.FindFirstValue("Id");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }


                var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "private");
                var userDir = Path.Combine(baseDir, userId);

                if (!Directory.Exists(userDir))
                {
                    Directory.CreateDirectory(userDir);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(userDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }


                return Ok(new { message = "File uploaded successfully", path = fileName });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpGet("{image}")]
        [Authorize]
        public async Task<IActionResult> ActionName(string image)
        {
            try
            {
                var User = _accessor?.HttpContext?.User;
                var userId = User?.FindFirstValue("Id");
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "private", userId, image);

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"Image not found with path {filePath}");
                    return NotFound(new { message = "Image not found" });
                }

                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(filePath, out string? contentType))
                {
                    contentType = "application/octet-stream";
                }

                return PhysicalFile(filePath, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

    }
}