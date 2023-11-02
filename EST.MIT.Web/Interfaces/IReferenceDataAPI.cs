using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IReferenceDataAPI
{
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string InvoiceType);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? InvoiceType = null, string? Organisation = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetPaymentTypesAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetAccountsAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null, string? PaymentType = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetDeliveryBodiesAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null, string? PaymentType = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetFundsAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null, string? PaymentType = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetMarketingYearsAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null, string? PaymentType = null);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null, string? PaymentType = null);
}
