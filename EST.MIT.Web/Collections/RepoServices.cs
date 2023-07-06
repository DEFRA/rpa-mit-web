using Repositories;

namespace Collections;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepoServices(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
        services.AddSingleton<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddSingleton<IApprovalRepository, ApprovalRepository>();
        return services;
    }
}