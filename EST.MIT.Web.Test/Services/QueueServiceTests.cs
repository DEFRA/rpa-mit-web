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
        var logger = Mock.Of<ILogger<IEventQueueService>>();
        var queueClientMock = new Mock<QueueClient>();
        var eventQueueService = new EventQueueService(queueClientMock.Object, logger);
        var expectedMessageContent = "Expected error content";

        queueClientMock
            .Setup(qc => qc.SendMessageAsync(It.IsAny<string>()))
            .Callback(() => throw new RequestFailedException(expectedMessageContent));

        Exception exception = null;

        try
        {
            await eventQueueService.AddMessageToQueueAsync("message", "data");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception);
        Assert.Contains(expectedMessageContent, exception.Message);
    }
}