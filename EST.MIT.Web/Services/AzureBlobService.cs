using Azure.Storage.Blobs;

namespace EST.MIT.Web.Services;

public interface IAzureBlobService
{
    BlobServiceClient? blobServiceClient { get; set; }
}

public class AzureBlobService : IAzureBlobService
{
    private readonly IConfiguration? _configuration;
    private readonly BlobServiceClient _blobServiceClient;


    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;
        _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        blobServiceClient = _blobServiceClient;
    }

    public BlobServiceClient? blobServiceClient { get; set; }

}