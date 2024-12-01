namespace MinioWebApi.Configurations
{
    /// <summary>
    /// Representa as configurações necessárias para integrar o MinIO.
    /// </summary>
    public class MinioConfiguration
    {
        /// <summary>
        /// Endpoint do servidor MinIO.
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// Chave de acesso para autenticação no MinIO.
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Chave secreta para autenticação no MinIO.
        /// </summary>
        public string SecretKey { get; set; }
    }
}
