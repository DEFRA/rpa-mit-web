using Azure.Storage.Blobs;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs.Models;

namespace EST.MIT.Web.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly IConfiguration? _configuration;
    private readonly ILogger<IAzureBlobService> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobService(BlobServiceClient blobServiceClient, ILogger<IAzureBlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public BlobServiceClient? blobServiceClient { get; set; }

    public async Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file, string directory)
    {
        blobName = GetBlobUrl(directory, blobName);
        return await PostToBlobAsync(blobName, containerName, file);
    }

    public async Task<bool> AddFileToBlobAsync(string blobName, string containerName, IBrowserFile file) => await PostToBlobAsync(blobName, containerName, file);

    private BlobContainerClient GetBlobClient(string containerName) => _blobServiceClient.GetBlobContainerClient(containerName);
    private static string GetBlobUrl(string directory, string blobName) => $"{directory}/{blobName}";

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