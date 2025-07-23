using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Soskd.Infrastructure.Services;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("news")]
        public async Task<IActionResult> UploadNewsImages(IFormFile? smallPhoto, IFormFile? largePhoto)
        {
            if (smallPhoto == null)
                return BadRequest("Small photo is required.");

            var ext = Path.GetExtension(smallPhoto.FileName).ToLower();
            if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext) || smallPhoto.Length > 5_000_000)
                return BadRequest("Invalid small photo file.");

            var smallPath = await _fileService.SaveFileAsync(smallPhoto, "news");
            string? largePath = null;

            if (largePhoto != null)
            {
                var extLarge = Path.GetExtension(largePhoto.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extLarge) || largePhoto.Length > 5_000_000)
                    return BadRequest("Invalid large photo file.");

                largePath = await _fileService.SaveFileAsync(largePhoto, "news");
            }

            return Ok(new
            {
                smallPhotoUrl = smallPath,
                largePhotoUrl = largePath
            });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] string url)
        {
            var result = await _fileService.DeleteFileAsync(url);
            return result ? Ok("Deleted") : NotFound("File not found.");
        }
    }
}
