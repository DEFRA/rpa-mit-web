using Microsoft.Extensions.Logging;
using Azure;
using EST.MIT.Web.Services;
using Microsoft.Extensions.Configuration;

namespace EST.MIT.Web.Test.Services;

public class EventQueueServiceTests : TestContext
{

    private readonly Mock<ILogger<EventQueueService>> _mockLogger;
    private readonly Mock<IServiceBusProvider> _mockServiceBusProvider;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public EventQueueServiceTests()
    {
        _mockLogger = new Mock<ILogger<EventQueueService>>();
        _mockServiceBusProvider = new Mock<IServiceBusProvider>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(x => x["EventQueueName"]).Returns("QueueName");
    }

    [Fact]
    public async Task CreateMessage_ValidArguments_CallsSendMessageAsync()
    {
        var service = new EventQueueService(_mockServiceBusProvider.Object, _mockLogger.Object, _mockConfiguration.Object);
        var expectedMessageContent = "Expected error content";

        _mockServiceBusProvider
            .Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => throw new RequestFailedException(expectedMessageContent));

        Exception exception = null!;
        var success = false;

        try
        {
            success = await service.AddMessageToQueueAsync("message", "data");
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