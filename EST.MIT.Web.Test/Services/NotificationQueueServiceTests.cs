using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;
using Microsoft.Extensions.Logging;
using Azure;
using Microsoft.Extensions.Configuration;

namespace EST.MIT.Web.Test.Services;

public class NotificationQueueServiceTests
{
    private readonly Mock<ILogger<INotificationQueueService>> _mockLogger;
    private readonly Mock<IServiceBusProvider> _mockServiceBusProvider;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public NotificationQueueServiceTests()
    {
        _mockLogger = new Mock<ILogger<INotificationQueueService>>();
        _mockServiceBusProvider = new Mock<IServiceBusProvider>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(x => x["NotificationQueueName"]).Returns("QueueName");
    }

    [Fact]
    public async Task AddInvoiceApprovalNotification_ReturnsSuccess()
    {
        _mockServiceBusProvider.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var service = new NotificationQueueService(_mockServiceBusProvider.Object, _mockLogger.Object, _mockConfiguration.Object);
        var notification = new Notification(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), "CP", "approval", "email@defra.gov.uk", new NotificationInvoiceApprove { ApproverEmail = "approver@example.com" });

        await service.AddMessageToQueueAsync(notification);

        _mockServiceBusProvider.Verify(x => x.SendMessageAsync("QueueName", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddOutstandingApprovalNotification_ReturnsSuccess()
    {

        _mockServiceBusProvider.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var service = new NotificationQueueService(_mockServiceBusProvider.Object, _mockLogger.Object, _mockConfiguration.Object);
        var notification = new Notification(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), "CS", "approval", "email@defra.gov.uk", new NotificationOutstandingApproval { Name = "Invoice Name", Link = "http://defra.gov.uk", Value = "1000", InvoiceId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), SchemeType = "CS" });

        await service.AddMessageToQueueAsync(notification);

        _mockServiceBusProvider.Verify(x => x.SendMessageAsync("QueueName", It.IsAny<string>()), Times.Once);
    }
}
