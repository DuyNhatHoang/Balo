using Balo.Data.ViewModels;
using Balo.Extensions;
using Balo.Service.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Balo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _service;

        public FilesController(IFilesService service)
        {
            _service = service;
        }

     

        [HttpPost("UploadMinIO")]
        public async Task<IActionResult> UploadToMinIO()
        {
            try
            {
                var minioClient = MinIOExtentions.Instance();
                await _service.UploadFileMinIoAsync(null, minioClient);
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var minioClient = MinIOExtentions.Instance();
            var result = await _service.GetFileMinIoAsync(minioClient);
            if (result.Succeed)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }


    }
}
