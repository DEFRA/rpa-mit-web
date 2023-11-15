using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Repositories;

namespace EST.MIT.Web.Collections;

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