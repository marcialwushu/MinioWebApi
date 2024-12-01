
using Minio;
using Minio.DataModel.Args;

namespace MinioWebApi.Services
{
    /// <summary>
    /// Implementação do serviço para operações de arquivos no MinIO.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IMinioClient _minioClient;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="FileService"/>.
        /// </summary>
        /// <param name="minioClient">Cliente MinIO injetado.</param>
        public FileService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        /// <inheritdoc/>
        public async Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName)
        {
            var presignedUrl = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(60 * 60)) // 1 hora
                .ConfigureAwait(false);

            return presignedUrl;
        }

        /// <inheritdoc/>
        public async Task<bool> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType)
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType));

            return true;
        }
    }
}
