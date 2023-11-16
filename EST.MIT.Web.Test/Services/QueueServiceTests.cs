using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Queues;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Test.Services;

public class QueueServiceTests : TestContext
{

    [Fact]
    public async Task CreateMessage_ValidArguments_CallsSendMessageAsync()
    {
        Mock<ILogger<EventQueueService>> _mockLogger = new Mock<ILogger<EventQueueService>>();
        var queueClientMock = new Mock<QueueClient>();
        var eventQueueService = new EventQueueService(queueClientMock.Object, _mockLogger.Object);
        var expectedMessageContent = "Expected error content";

        queueClientMock
            .Setup(qc => qc.SendMessageAsync(It.IsAny<string>()))
            .Callback(() => throw new RequestFailedException(expectedMessageContent));

        Exception exception = null!;
        var success = false;

        try
        {
            success = await eventQueueService.AddMessageToQueueAsync("message", "data");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        var handledErrorMessage = $"Error {expectedMessageContent} sending \"message\" message to Event Queue.";

        Assert.False(success);
        Assert.Null(exception);

        _mockLogger.Verify(logger => logger.Log(
           LogLevel.Error,
           It.IsAny<EventId>(),
           It.Is<It.IsAnyType>((o, t) => string.Equals(handledErrorMessage, o.ToString())),
           null,
           (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>())
       );
    }
}