using Azure.Storage.Queues;

namespace Services;

public interface IAzureQueueService
{
    QueueServiceClient? queueServiceClient { get; set; }
}

public class AzureQueueService : IAzureQueueService
{
    private readonly IConfiguration? _configuration;
    private readonly QueueServiceClient? _queueServiceClient;


    public AzureQueueService(IConfiguration configuration)
    {
        _configuration = configuration;
        _queueServiceClient = new QueueServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        queueServiceClient = _queueServiceClient;
    }

    public QueueServiceClient? queueServiceClient { get; set; }

}