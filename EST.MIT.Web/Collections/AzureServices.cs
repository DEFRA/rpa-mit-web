using System.Diagnostics.CodeAnalysis;
using Services;

namespace Collections;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton<IQueueService, QueueService>();
        services.AddSingleton<IAzureBlobService, AzureBlobService>();
        services.AddSingleton<IAzureQueueService, AzureQueueService>();
        return services;
    }
}