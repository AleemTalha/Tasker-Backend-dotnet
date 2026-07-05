using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NameController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _accessor;

        public NameController(AppDbContext db, IHttpContextAccessor accessor)
        {
            _db = db;
            _accessor = accessor;
        }

        [HttpPost("")]
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


                var uploadDir = Path.Combine("uploads", "private");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);

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
    }
}