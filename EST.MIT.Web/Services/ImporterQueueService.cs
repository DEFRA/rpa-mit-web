using System.Text;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class ImporterQueueService : IImporterQueueService
{
    private readonly ILogger<IImporterQueueService> _logger;
    private readonly QueueClient _queueClient;

    public ImporterQueueService(QueueClient queueClient, ILogger<IImporterQueueService> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task<bool> AddMessageToQueueAsync(ImportRequest request)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(request.ToMessage());
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
            _logger.LogError($"Error {ex.Message} sending message for File {request.FileName} from {request.CreatedBy} to Invoice Importer Queue.");
            return false;
        }

    }
}