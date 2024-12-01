
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

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

        public async Task<bool> CreateBucket(string bucketName, CancellationToken cancellationToken)
        {
            try
            {
                // Cria o argumento para verificar se o bucket existe
                var bucketExistsArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                // Create bucket if it doesn't exist.
                bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
                if (found)
                {
                    Console.WriteLine($"{bucketName} already exists");
                    return true;
                }
                else
                {
                    // Cria o argumento para criar o bucket
                    var makeBucketArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);

                    // Cria o bucket
                    await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                    Console.WriteLine($"{bucketName} is created successfully");
                    return true;
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e.Message);
                return false;
            }
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

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _minioClient.ListBucketsAsync(cancellationToken);
                Console.WriteLine("MinIO connection successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to MinIO: {ex.Message}");
                return false;
            }
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
