using System.Diagnostics.CodeAnalysis;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Services;

namespace EST.MIT.Web.Collections;

[ExcludeFromCodeCoverage]
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceAPI, InvoiceAPI>();
        services.AddScoped<IApprovalAPI, ApprovalAPI>();
        services.AddScoped<IReferenceDataAPI, ReferenceDataAPI>();
        return services;
    }
}