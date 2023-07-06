using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Forms;
using Azure;
using Azure.Storage.Queues;
using FluentAssertions;
using Azure.Storage.Queues.Models;

namespace Services.Tests;

public class QueueServiceTests : TestContext
{

    private readonly IConfiguration _configuration;
    private readonly QueueService _queueService;
    private Mock<QueueClient> mockQueueClient;
    private ILogger<QueueService> _logger;

    public QueueServiceTests()
    {
        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(x => x.Value).Returns("connection_string");
        configMock.Setup(x => x.GetSection(It.Is<string>(s => s == "ConnectionStrings:PrimaryConnection"))).Returns(configSectionMock.Object);
        _configuration = configMock.Object;

        _logger = Mock.Of<ILogger<QueueService>>();

        var mockQueueServiceClient = new Mock<QueueServiceClient>();
        mockQueueClient = new Mock<QueueClient>();
        var mockAzureQueueService = new Mock<IAzureQueueService>();

        mockAzureQueueService.Setup(x => x.queueServiceClient).Returns(mockQueueServiceClient.Object);
        mockQueueClient.Setup(x => x.CreateIfNotExistsAsync(null, default)).ReturnsAsync(new Mock<Response>().Object);
        mockQueueClient.Setup(x => x.SendMessageAsync(It.IsAny<string>())).ReturnsAsync(new Mock<Response<SendReceipt>>().Object);
        mockQueueServiceClient.Setup(x => x.GetQueueClient(It.IsAny<string>())).Returns(mockQueueClient.Object);
        _queueService = new QueueService(_configuration, _logger, mockAzureQueueService.Object);

    }

    [Fact]
    public async void AddMessageToQueueAsync_Successful()
    {
        string queueName = "test-queue";
        string message = "test-message";

        mockQueueClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

        var result = await _queueService.AddMessageToQueueAsync(queueName, message);

        result.Should().BeTrue();
    }

    [Fact]
    public async void AddMessageToQueueAsync_Queue_Not_Exists()
    {
        string queueName = "test-queue";
        string message = "test-message";

        mockQueueClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

        var result = await _queueService.AddMessageToQueueAsync(queueName, message);

        result.Should().BeFalse();
    }

}