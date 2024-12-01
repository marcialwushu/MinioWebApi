# MinIO Web API

Este projeto é uma Web API construída em C# com integração ao MinIO. Ele permite gerar URLs pre-assinadas para download de arquivos e fazer upload de arquivos em buckets do MinIO. A arquitetura do projeto segue boas práticas, com separação de responsabilidades, uso de injeção de dependências e configuração centralizada.

---

## Tecnologias Utilizadas

- **.NET 8**
- **MinIO .NET SDK**
- **Injeção de Dependências**
- **Configuração via `appsettings.json`**

---

## Funcionalidades

1. **Gerar URL Pre-assinada**:
   Permite criar uma URL válida por tempo limitado para download de objetos armazenados em buckets do MinIO.

2. **Upload de Arquivos**:
   Realiza upload de arquivos diretamente para um bucket no MinIO.

---

## Estrutura do Projeto

```plaintext
MinioWebApi
├── Controllers
│   ├── FileController.cs          // Controlador da API
├── Services
│   ├── IFileService.cs            // Interface do serviço de arquivos
│   ├── FileService.cs             // Implementação do serviço de arquivos
├── Models
│   ├── FileUploadResponse.cs      // Modelo de resposta de upload
├── Configurations
│   ├── MinioConfiguration.cs      // Configuração do MinIO
├── Program.cs                     // Inicialização do projeto
├── appsettings.json               // Configurações da aplicação
