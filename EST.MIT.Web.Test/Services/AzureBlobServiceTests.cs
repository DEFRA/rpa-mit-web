using EST.MIT.Web.Services;
using Microsoft.Extensions.Configuration;

namespace EST.MIT.Web.Test.Services;

public class AzureBlobServiceTests
{
    [Fact]
    public void BlobServiceClient_ShouldNotBeNull()
    {
        var _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:PrimaryConnection", "AccountName=testaccount;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;"
                }
            })
            .Build();

        var azureBlobService = new AzureBlobService(_configuration);

        var blobServiceClient = azureBlobService.blobServiceClient;

        blobServiceClient.Should().NotBeNull();
    }
}