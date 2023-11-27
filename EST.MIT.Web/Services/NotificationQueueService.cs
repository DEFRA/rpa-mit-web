using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Services;

public class NotificationQueueService : INotificationQueueService
{
    private readonly ILogger<INotificationQueueService> _logger;
    private readonly QueueClient _queueClient;

    public NotificationQueueService(QueueClient queueClient, ILogger<INotificationQueueService> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task<bool> AddMessageToQueueAsync(Notification request)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));

            return true;

        }
        catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueAlreadyExists)
        {
            // Ignore any errors if the queue already exists

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message} sending \"{request}\" message to Notification Queue.");
            return false;
        }
    }
}