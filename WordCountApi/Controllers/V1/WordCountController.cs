using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordCountApi.Services.Interfaces;

namespace WordCountApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    public class WordCountController : ControllerBase
    {
        private readonly IWordCountService _service;
        private readonly ILogger<WordCountController> _logger;

        public WordCountController(IWordCountService service, ILogger<WordCountController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: empty file received.");
                    return BadRequest("File is empty or not provided.");
                }

                //  Validate file extension
                if (Path.GetExtension(file.FileName)?.ToLower() != ".txt")
                {
                    _logger.LogWarning("Upload failed: unsupported file type {FileName}", file.FileName);
                    return BadRequest("Only .txt files are allowed.");
                }

                // Optional: Validate MIME type (browser-dependent)
                if (file.ContentType != "text/plain")
                {
                    _logger.LogWarning("Upload failed: invalid content type {ContentType}", file.ContentType);
                    return BadRequest("Invalid file content type. Only 'text/plain' is allowed.");
                }

                using var stream = file.OpenReadStream();
                var result = await _service.CountWordsAsync(stream);

                _logger.LogInformation("File processed successfully. Total words: {Count}", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
