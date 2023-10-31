using EST.MIT.Web.Services;

namespace EST.MIT.Web.Collections;

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