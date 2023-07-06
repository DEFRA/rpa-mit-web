using System.Diagnostics.CodeAnalysis;
using Services;

namespace Collections;

[ExcludeFromCodeCoverageAttribute]
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