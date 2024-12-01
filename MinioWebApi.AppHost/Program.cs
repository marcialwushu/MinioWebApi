
var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.MinioWebApi>("miniowebapi");

builder.Build().Run();
