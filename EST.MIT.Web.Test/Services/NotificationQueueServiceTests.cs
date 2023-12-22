using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure;

namespace EST.MIT.Web.Test.Services;

public class NotificationQueueServiceTests
{
    private readonly Mock<QueueClient> mockQueueClient;
    private readonly Mock<ILogger<INotificationQueueService>> mockLogger;
    private NotificationQueueService service;

    public NotificationQueueServiceTests()
    {
        mockQueueClient = new Mock<QueueClient>();
        mockLogger = new Mock<ILogger<INotificationQueueService>>();
        service = new NotificationQueueService(mockQueueClient.Object, mockLogger.Object);

        mockQueueClient.Setup(q => q.SendMessageAsync(It.IsAny<string>(), null, null, CancellationToken.None))
            .ReturnsAsync(Response.FromValue(It.IsAny<SendReceipt>(), new Mock<Response>().Object));
    }

    [Fact]
    public async Task AddInvoiceApprovalNotification_ReturnsSuccess()
    {
        var notification = new Notification(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), "CP", "approval", "email@defra.gov.uk", new NotificationInvoiceApprove { ApproverEmail = "approver@example.com" });

        mockQueueClient.Setup(q => q.SendMessageAsync(It.IsAny<string>(), null, null, CancellationToken.None))
            .ReturnsAsync(Response.FromValue(It.IsAny<SendReceipt>(), new Mock<Response>().Object));

        var result = await service.AddMessageToQueueAsync(notification);

        Assert.True(result);
    }

    [Fact]
    public async Task AddOutstandingApprovalNotification_ReturnsSuccess()
    {
        var notification = new Notification(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), "CS", "approval", "email@defra.gov.uk", new NotificationOutstandingApproval { Name = "Invoice Name", Link = "http://defra.gov.uk", Value = "1000", InvoiceId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296").ToString(), SchemeType = "CS" });

        mockQueueClient.Setup(q => q.SendMessageAsync(It.IsAny<string>(), null, null, CancellationToken.None))
            .ReturnsAsync(Response.FromValue(It.IsAny<SendReceipt>(), new Mock<Response>().Object));

        var result = await service.AddMessageToQueueAsync(notification);

        Assert.True(result);
    }
}
