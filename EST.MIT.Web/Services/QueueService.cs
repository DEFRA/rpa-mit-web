using System.Text;
using Azure.Storage.Queues;

namespace EST.MIT.Web.Services;

public interface IQueueService
{
    Task<bool> AddMessageToQueueAsync(string queueName, string message);
}

public class QueueService : IQueueService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueueService> _logger;
    private readonly QueueServiceClient _queueServiceClient;

    public QueueService(IConfiguration configuration, ILogger<QueueService> logger, IAzureQueueService azureQueueService)
    {
        _configuration = configuration;
        _logger = logger;
        _queueServiceClient = azureQueueService.queueServiceClient == null
                                ? new QueueServiceClient(_configuration.GetConnectionString("PrimaryConnection"))
                                : azureQueueService.queueServiceClient;
    }

    private QueueClient GetQueueClient(string queueName) => _queueServiceClient.GetQueueClient(queueName);

    public async Task<bool> AddMessageToQueueAsync(string queueName, string message)
    {
        var client = GetQueueClient(queueName);
        await client.CreateIfNotExistsAsync();

        if (!await client.ExistsAsync())
        {
            _logger.LogInformation("Queue {queueName} not found!", queueName);
            return false;
        }

        await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
        return true;
    }

}