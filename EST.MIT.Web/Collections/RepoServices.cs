using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Repositories;

namespace EST.MIT.Web.Collections;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepoServices(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddScoped<IApprovalRepository, ApprovalRepository>();
        return services;
    }
}