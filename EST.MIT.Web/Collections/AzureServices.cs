using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Collections;
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAzureBlobService>(_ =>
            {
                var blobStorageAccountCredential = configuration.GetSection("BlobConnectionString:Credential").Value;
                var blobContainerName = configuration.GetSection("BlobContainerName").Value;
                blobContainerName = string.IsNullOrWhiteSpace(blobContainerName) ? AzureBlobService.default_BlobContainerName : blobContainerName;
                var logger = _.GetService<ILogger<IAzureBlobService>>();
                if (logger is null)
                {
                    return null;
                }
                if (IsManagedIdentity(blobStorageAccountCredential))
                {
                    var blobServiceUri = new Uri(configuration.GetSection("BlobConnectionString:BlobServiceUri").Value);
                    Console.WriteLine($"Startup.BlobClient using Managed Identity with url {blobServiceUri}");
                    return new AzureBlobService(new BlobServiceClient(blobServiceUri, new DefaultAzureCredential()), logger, blobContainerName);
                }
                else
                {
                    return new AzureBlobService(new BlobServiceClient(configuration.GetSection("BlobConnectionString").Value), logger, blobContainerName);
                }
            });

        services.AddSingleton<IEventQueueService>(_ =>
        {
            var eventQueueName = configuration.GetSection("EventQueueName").Value;
            var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
            var logger = _.GetService<ILogger<IEventQueueService>>();
            if (logger is null)
            {
                return null;
            }
            if (IsManagedIdentity(queueStorageAccountCredential))
            {
                var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                var queueUrl = new Uri($"{queueServiceUri}{eventQueueName}");
                return new EventQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()), logger);
            }
            else
            {
                return new EventQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, eventQueueName), logger);
            }
        });

        services.AddSingleton<IImporterQueueService>(_ =>
        {
            var importerQueueName = configuration.GetSection("ImporterQueueName").Value;
            var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
            var logger = _.GetService<ILogger<IImporterQueueService>>();
            if (logger is null)
            {
                return null;
            }
            if (IsManagedIdentity(queueStorageAccountCredential))
            {
                var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                var queueUrl = new Uri($"{queueServiceUri}{importerQueueName}");
                return new ImporterQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()), logger);
            }
            else
            {
                return new ImporterQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, importerQueueName), logger);
            }
        });

        services.AddSingleton<INotificationQueueService>(_ =>
        {
            var notificationQueueName = configuration.GetSection("NotificationQueueName").Value;
            var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
            var logger = _.GetService<ILogger<INotificationQueueService>>();
            if (logger is null)
            {
                return null;
            }
            if (IsManagedIdentity(queueStorageAccountCredential))
            {
                var serviceBusNamespace = configuration.GetSection("QueueConnectionString:FullyQualifiedNamespace").Value;
                Console.WriteLine($"Startup.ServiceBusClient using Managed Identity with namespace {serviceBusNamespace}");
                var serviceBusClient = new ServiceBusClient(serviceBusNamespace, new DefaultAzureCredential());
                return new NotificationQueueService(serviceBusClient, logger);
            }
            else
            {
                var serviceBusConnectionString = configuration.GetSection("QueueConnectionString").Value;
                var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
                return new NotificationQueueService(serviceBusClient, logger);
            }
        });

        return services;
    }

    private static bool IsManagedIdentity(string credentialName)
    {
        return credentialName != null && credentialName.ToLower() == "managedidentity";
    }
}