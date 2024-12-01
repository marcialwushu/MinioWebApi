using Microsoft.AspNetCore.Mvc;
using MinioWebApi.Services;
using MinioWebApi.Metrics;
using System.Security.AccessControl;

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
        private readonly ILogger<FileController> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="FileController"/>.
        /// </summary>
        /// <param name="fileService">Serviço de arquivos injetado.</param>
        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Inicia o temporizador
            _logger.LogInformation("Starting GeneratePresignedUrl for bucket '{BucketName}' and object '{ObjectName}'.", bucketName, objectName);

            try
            {
                var url = await _fileService.GeneratePresignedUrlAsync(bucketName, objectName);

                stopwatch.Stop(); // Para o temporizador
                var processingTime = stopwatch.Elapsed.TotalSeconds; // Calcula o tempo em segundos

                // Registra as métricas customizadas
                CustomMetrics.IncrementRequest();
                CustomMetrics.RecordDuration(processingTime);

                _logger.LogInformation("Presigned URL generated successfully for bucket '{BucketName}' and object '{ObjectName}' in {ProcessingTime} seconds.", bucketName, objectName, processingTime);

                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating presigned URL for bucket '{BucketName}' and object '{ObjectName}'.", bucketName, objectName);
                return StatusCode(500, new { Message = "An error occurred while generating the presigned URL." });
            }
            
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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Inicia o temporizador
            _logger.LogInformation("Starting GeneratePresignedUrl for bucket '{BucketName}' and object '{ObjectName}'.", file.FileName, bucketName);

            try
            {
                using var stream = file.OpenReadStream();
                var isUploaded = await _fileService.UploadFileAsync(bucketName, file.FileName, stream, file.ContentType);

                stopwatch.Stop(); // Para o temporizador
                var processingTime = stopwatch.Elapsed.TotalSeconds; // Calcula o tempo em segundos

                _logger.LogInformation("Presigned URL generated successfully for bucket '{BucketName}' and object '{ObjectName}' in {ProcessingTime} seconds.", bucketName, file.FileName, processingTime);

                // Registra as métricas customizadas
                CustomMetrics.IncrementRequest();
                CustomMetrics.RecordDuration(processingTime);

                if (!isUploaded)
                    return BadRequest("File upload failed.");

                return Ok(new { Message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating presigned URL for bucket '{BucketName}' and object '{ObjectName}'.", bucketName, file.FileName);
                return StatusCode(500, new { Message = "An error occurred while generating the presigned URL." });
            }
            
        }
    }
}
