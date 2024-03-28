using System.Text.Json;
using Azure.Messaging.ServiceBus;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Services;

public class EventQueueService : IEventQueueService
{
    private readonly ILogger<IEventQueueService> _logger;
    private readonly IServiceBusProvider _serviceBusProvider;
    private readonly IConfiguration _configuration;

    public EventQueueService(IServiceBusProvider serviceBusProvider, ILogger<IEventQueueService> logger, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> AddMessageToQueueAsync(string message, string data)
    {
        var eventRequest = new Event()
        {
            Name = "Payments",
            Properties = new EventProperties()
            {
                Status = "",
                Checkpoint = "",
                Action = new EventAction()
                {
                    Type = "",
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Data = data
                }
            }
        };

        try
        {
            await _serviceBusProvider.SendMessageAsync(_configuration.GetSection("EventQueueName").Value ,JsonSerializer.Serialize(eventRequest));
            return true;

        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            // Ignore any errors if the queue already exists
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message} sending \"{message}\" message to Event Queue.");
            return false;
        }
    }
}