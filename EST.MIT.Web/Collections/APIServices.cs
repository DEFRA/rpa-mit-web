using System.Diagnostics.CodeAnalysis;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Collections;

[ExcludeFromCodeCoverage]
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceAPI, InvoiceAPI>();
        services.AddSingleton<IApprovalAPI, ApprovalAPI>();
        services.AddSingleton<IReferenceDataAPI, ReferenceDataAPI>();
        return services;
    }
}