using System.Text.Json;
using Azure.Messaging.ServiceBus;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class NotificationQueueService : INotificationQueueService
{
    private readonly ILogger<INotificationQueueService> _logger;
    private readonly IServiceBusProvider _serviceBusProvider;
    private readonly IConfiguration _configuration;

    public NotificationQueueService(IServiceBusProvider serviceBusProvider, ILogger<INotificationQueueService> logger, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> AddMessageToQueueAsync(Notification request)
    {
        try
        {
            await _serviceBusProvider.SendMessageAsync(_configuration.GetSection("NotificationQueueName").Value, JsonSerializer.Serialize(request));
            return true;
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
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