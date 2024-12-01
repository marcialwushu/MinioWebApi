using Microsoft.AspNetCore.Mvc;
using MinioWebApi.Services;

namespace MinioWebApi.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações relacionadas a arquivos no MinIO.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="FileController"/>.
        /// </summary>
        /// <param name="fileService">Serviço de arquivos injetado.</param>
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Gera uma URL pre-assinada para download de um arquivo no MinIO.
        /// </summary>
        /// <param name="bucketName">Nome do bucket.</param>
        /// <param name="objectName">Nome do objeto.</param>
        /// <returns>Retorna uma URL pre-assinada válida por tempo limitado.</returns>
        [HttpGet("generate-url")]
        public async Task<IActionResult> GeneratePresignedUrl(string bucketName, string objectName)
        {
            var url = await _fileService.GeneratePresignedUrlAsync(bucketName, objectName);
            return Ok(new { Url = url });
        }

        /// <summary>
        /// Realiza o upload de um arquivo para o MinIO.
        /// </summary>
        /// <param name="file">Arquivo a ser enviado.</param>
        /// <param name="bucketName">Nome do bucket.</param>
        /// <returns>Retorna uma mensagem de sucesso ou falha.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string bucketName)
        {
            using var stream = file.OpenReadStream();
            var isUploaded = await _fileService.UploadFileAsync(bucketName, file.FileName, stream, file.ContentType);

            if (!isUploaded)
                return BadRequest("File upload failed.");

            return Ok(new { Message = "File uploaded successfully." });
        }
    }
}
