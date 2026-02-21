using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CRM.Services.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "image-uploads";

        public AzureBlobService(IConfiguration configuration)
        {
            string storageConnectionString = configuration.GetSection("AzureStorage:ConnectionString").Value;
            _blobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, Dictionary<string, string> metadata = null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

            // Set metadata if provided
            var metadataDictionary = metadata ?? new Dictionary<string, string>();

            await blobClient.UploadAsync(imageStream, blobHttpHeaders);
            await blobClient.SetMetadataAsync(metadataDictionary);

            return blobClient.Uri.ToString();
        }

    }
}
