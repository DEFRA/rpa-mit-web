using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure;

namespace EST.MIT.Web.Test.Services;

public class ImporterQueueServiceTests
{
    private readonly Mock<ILogger<ImporterQueueService>> _mockLogger;
    private readonly Mock<QueueClient> queueClientMock;
    public ImporterQueueServiceTests()
    {
        _mockLogger = new Mock<ILogger<ImporterQueueService>>();
        queueClientMock = new Mock<QueueClient>();
    }

    [Fact]
    public async Task AddMessageToQueueAsync_ValidRequest_ReturnsSuccess()
    {
        var service = new ImporterQueueService(queueClientMock.Object, _mockLogger.Object);

        queueClientMock.Setup(q => q.SendMessageAsync(It.IsAny<string>(), null, null, CancellationToken.None))
            .ReturnsAsync(Response.FromValue(It.IsAny<SendReceipt>(), new Mock<Response>().Object));

        var result = await service.AddMessageToQueueAsync(new ImportRequest { FileName = "invoice_12345.csv", CreatedBy = "user1" });

        Assert.True(result);
    }
}