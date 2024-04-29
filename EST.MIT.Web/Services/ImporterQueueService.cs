using System.Text;
using Azure;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace EST.MIT.Web.Services;

public class ImporterQueueService : IImporterQueueService
{
    private readonly ILogger<IImporterQueueService> _logger;
    private readonly IServiceBusProvider _serviceBusProvider;
    private readonly IConfiguration _configuration;

    public ImporterQueueService(IServiceBusProvider serviceBusProvider, ILogger<IImporterQueueService> logger, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> AddMessageToQueueAsync(ImportRequest request)
    {
        try
        {
            await _serviceBusProvider.SendMessageAsync(_configuration.GetSection("ImporterQueueName").Value, JsonSerializer.Serialize<ImportRequest>(request));
            return true;

        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            // Ignore any errors if the queue already exists
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message} sending message for File {request.FileName} from {request.CreatedBy} to Invoice Importer Queue.");
            return false;
        }
    }
}