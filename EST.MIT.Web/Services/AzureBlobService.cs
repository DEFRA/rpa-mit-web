using Azure.Storage.Blobs;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs.Models;

namespace EST.MIT.Web.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly ILogger<IAzureBlobService> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;

    public const string default_BlobContainerName = "rpa-mit-invoices";

    public AzureBlobService(BlobServiceClient blobServiceClient, ILogger<IAzureBlobService> logger, string blobContainerName)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _blobContainerName = blobContainerName;
    }

    public BlobServiceClient? blobServiceClient { get; set; }

    public async Task<bool> AddFileToBlobAsync(string blobName, IBrowserFile file) => await PostToBlobAsync(blobName, file);

    private BlobContainerClient GetBlobClient(string containerName) => _blobServiceClient.GetBlobContainerClient(containerName);

    private async Task<bool> PostToBlobAsync(string blobName, IBrowserFile file)
    {
        var container = GetBlobClient(_blobContainerName);
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