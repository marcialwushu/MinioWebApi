using Microsoft.Extensions.Options;
using Minio;
using MinioWebApi.Configurations;
using MinioWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do MinIO
builder.Services.Configure<MinioConfiguration>(builder.Configuration.GetSection("Minio"));
builder.Services.AddSingleton<IMinioClient>(provider =>
{
    var config = provider.GetRequiredService<IOptions<MinioConfiguration>>().Value;
    return new MinioClient()
        .WithEndpoint(config.Endpoint)
        .WithCredentials(config.AccessKey, config.SecretKey)
        .Build();
});

builder.AddServiceDefaults();

builder.Services.AddScoped<IFileService, FileService>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
