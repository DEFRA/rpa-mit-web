using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Interfaces;

public interface IReferenceDataAPI
{
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string InvoiceType);
    Task<ApiResponse<IEnumerable<SchemeType>>> GetSchemeTypesAsync(string? InvoiceType = null, string? Organisation = null);
    Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null);
    Task<ApiResponse<IEnumerable<MainAccount>>> GetAccountsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<ApiResponse<IEnumerable<DeliveryBody>>> GetDeliveryBodiesAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<ApiResponse<IEnumerable<FundCode>>> GetFundsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<ApiResponse<IEnumerable<MarketingYear>>> GetMarketingYearsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemesAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType);
}
