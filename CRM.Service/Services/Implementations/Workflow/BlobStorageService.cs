using Azure.Storage.Blobs;

namespace CRM.Services.Implementations.Workflow;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _container;

    //public BlobStorageService(IConfiguration configuration)
    //{
    //    var connectionString = configuration["AzureBlob:ConnectionString"]
    //        ?? throw new InvalidOperationException("AzureBlob:ConnectionString is not configured.");
    //    var containerName = configuration["AzureBlob:ContainerName"] ?? "crm-files";
    //    _container = new BlobContainerClient(connectionString, containerName);
    //}

    //public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    //{
    //    await _container.CreateIfNotExistsAsync(PublicAccessType.None);
    //    var blobName = $"{Guid.NewGuid()}/{fileName}";
    //    var blob = _container.GetBlobClient(blobName);
    //    await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
    //    return blob.Uri.ToString();
    //}

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {

        return string.Empty;
    }
}
