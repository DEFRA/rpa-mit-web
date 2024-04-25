using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EST.MIT.Web.Test.Services;

public class ImporterQueueServiceTests
{
    private readonly Mock<ILogger<ImporterQueueService>> _mockLogger;
    private readonly Mock<IServiceBusProvider> _mockServiceBusProvider;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public ImporterQueueServiceTests()
    {
        _mockLogger = new Mock<ILogger<ImporterQueueService>>();
        _mockServiceBusProvider = new Mock<IServiceBusProvider>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(x => x["ImporterQueueName"]).Returns("QueueName");
    }

    [Fact]
    public async Task AddMessageToQueueAsync_ValidRequest_ReturnsSuccess()
    {
        _mockServiceBusProvider.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var service = new ImporterQueueService(_mockServiceBusProvider.Object, _mockLogger.Object, _mockConfiguration.Object);

        await service.AddMessageToQueueAsync(new ImportRequest { FileName = "invoice_12345.csv", CreatedBy = "user1" });

        _mockServiceBusProvider.Verify(x => x.SendMessageAsync("QueueName", It.IsAny<string>()), Times.Once);
    }
}