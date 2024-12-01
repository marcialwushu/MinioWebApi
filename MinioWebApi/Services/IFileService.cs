namespace MinioWebApi.Services
{
    /// <summary>
    /// Define a interface para operações relacionadas a arquivos no MinIO.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Gera uma URL pre-assinada para download de um objeto no MinIO.
        /// </summary>
        /// <param name="bucketName">Nome do bucket.</param>
        /// <param name="objectName">Nome do objeto.</param>
        /// <returns>Uma URL pre-assinada válida por tempo limitado.</returns>
        Task<string> GeneratePresignedUrlAsync(string bucketName, string objectName);

        /// <summary>
        /// Realiza o upload de um arquivo para o MinIO.
        /// </summary>
        /// <param name="bucketName">Nome do bucket.</param>
        /// <param name="objectName">Nome do arquivo/objeto.</param>
        /// <param name="fileStream">Stream do arquivo a ser enviado.</param>
        /// <param name="contentType">Tipo de conteúdo (MIME type).</param>
        /// <returns>Retorna um valor booleano indicando sucesso ou falha.</returns>
        Task<bool> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType);

        Task<bool> CreateBucket(string bucketName, CancellationToken cancellationToken);

        Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
    }
}
