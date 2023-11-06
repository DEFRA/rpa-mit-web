using Azure.Identity;
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
                var logger = _.GetService<ILogger<IAzureBlobService>>();
                if (IsManagedIdentity(blobStorageAccountCredential))
                {
                    var blobServiceUri = new Uri(configuration.GetSection("BlobConnectionString:BlobServiceUri").Value);
                    Console.WriteLine($"Startup.BlobClient using Managed Identity with url {blobServiceUri}");
                    return new AzureBlobService(new BlobServiceClient(blobServiceUri, new DefaultAzureCredential()), logger);
                }
                else
                {
                    return new AzureBlobService(new BlobServiceClient(configuration.GetSection("BlobConnectionString").Value), logger);
                }
            });

        services.AddSingleton<IEventQueueService>(_ =>
        {
            var eventQueueName = configuration.GetSection("EventQueueName").Value;
            var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
            var logger = _.GetService<ILogger<IEventQueueService>>();

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

        return services;
    }

    private static bool IsManagedIdentity(string credentialName)
    {
        return credentialName != null && credentialName.ToLower() == "managedidentity";
    }
}