using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;

namespace EST.MIT.Web.Interfaces;

public interface IAzureBlobService
{
    BlobServiceClient? blobServiceClient { get; set; }
    Task<bool> AddFileToBlobAsync(string blobName, IBrowserFile file);

}
