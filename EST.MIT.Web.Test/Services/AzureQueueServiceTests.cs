using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Services.Tests;

public class AzureQueueServiceTests
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

        var azureQueueService = new AzureQueueService(_configuration);

        var queueServiceClient = azureQueueService.queueServiceClient;

        queueServiceClient.Should().NotBeNull();
    }
}