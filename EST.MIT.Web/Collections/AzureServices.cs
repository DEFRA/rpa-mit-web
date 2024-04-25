using Azure.Identity;
using Azure.Storage.Blobs;
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
                var serviceBusNamespace = configuration.GetSection("QueueConnectionString:FullyQualifiedNamespace").Value;
                Console.WriteLine($"Event Service using Managed Identity with namespace {serviceBusNamespace}");
                return new EventQueueService(new ServiceBusProvider(configuration), logger, configuration);
            }
            else
            {
                return new EventQueueService(new ServiceBusProvider(configuration), logger, configuration);
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
                var serviceBusNamespace = configuration.GetSection("QueueConnectionString:FullyQualifiedNamespace").Value;
                Console.WriteLine($"Importer Service using Managed Identity with namespace {serviceBusNamespace}");
                return new ImporterQueueService(new ServiceBusProvider(configuration), logger, configuration);
            }
            else
            {
                return new ImporterQueueService(new ServiceBusProvider(configuration), logger, configuration);
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
                Console.WriteLine($"Notification Service using Managed Identity with namespace {serviceBusNamespace}");
                return new NotificationQueueService(new ServiceBusProvider(configuration), logger, configuration);
            }
            else
            {
                return new NotificationQueueService(new ServiceBusProvider(configuration), logger, configuration);
            }
        });

        return services;
    }

    private static bool IsManagedIdentity(string credentialName)
    {
        return credentialName != null && credentialName.ToLower() == "managedidentity";
    }
}