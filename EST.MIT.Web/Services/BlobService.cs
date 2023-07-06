using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Services;

public interface IBlobService
{
    Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file, string directory);
    Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file);
}

public class BlobService : IBlobService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobService> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(IConfiguration configuration, ILogger<BlobService> logger, IAzureBlobService azureBlobService)
    {
        _configuration = configuration;
        _logger = logger;
        _blobServiceClient = azureBlobService.blobServiceClient == null
                                ? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"))
                                : azureBlobService.blobServiceClient;
    }

    private BlobContainerClient GetBlobClient(string containerName) => _blobServiceClient.GetBlobContainerClient(containerName);
    private static string GetBlobUrl(string directory, string blobName) => $"{directory}/{blobName}";


    public async Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file, string directory)
    {
        blobName = GetBlobUrl(directory, blobName);
        return await PostToBlobAsync(blobName, containerName, file);
    }

    public async Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file) => await PostToBlobAsync(blobName, containerName, file);

    private async Task<bool> PostToBlobAsync(string blobName, string containerName, IBrowserFile file)
    {
        var container = GetBlobClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

        if (!await container.ExistsAsync())
        {
            _logger.LogError("Container {blobName} was not found!", blobName);
            return false;
        }

        //1+e7 is the max size of a blob
        await container.UploadBlobAsync(blobName, file.OpenReadStream((long)1e+7));
        _logger.LogInformation("File {blobName} uploaded successfully", blobName);
        return true;
    }


}