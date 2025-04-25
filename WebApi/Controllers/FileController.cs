using ClassLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IMinioService _minioService;
        public FileController(IMinioService minioService)
        {
            _minioService = minioService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile formFile)
        {
            //read
            var fileStream = formFile.OpenReadStream();
            var fileName = formFile.FileName;
            var fileSize = formFile.Length;
            var contentType = formFile.ContentType;
            //upload
            var result = await _minioService.PutObject(fileStream, fileName, contentType, fileSize);
            //return object dto
            return Created(
                "$http://localhost:5124/api/files/{result}",
                new FileResponseDto() { 
                    ObjectId = result,
                    FileName = fileName,
                    FileSize = fileSize,
                    FileType = contentType,
                    FilePath = $"http://localhost:5124/api/files/{result}"
                });
        
        }

        [HttpGet("{result}")]
        public async Task<IActionResult> GetFile(string result)
        { 
            var response = await _minioService.GetObjectResponse(result);
            return File(response.Data, response.ContentType);
        }
    }
}
