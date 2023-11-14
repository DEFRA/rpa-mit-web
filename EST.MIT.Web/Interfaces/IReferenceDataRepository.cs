namespace EST.MIT.Web.Interfaces;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetOrganisationsListAsync();
    Task<HttpResponseMessage> GetSchemeTypesListAsync(string? InvoiceType = null, string? Organisation = null);
    Task<HttpResponseMessage> GetPaymentTypesListAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null);
    Task<HttpResponseMessage> GetAccountsListAsync
       (string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<HttpResponseMessage> GetDeliveryBodiesListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<HttpResponseMessage> GetFundsListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<HttpResponseMessage> GetMarketingYearsListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType);
    Task<HttpResponseMessage> GetSchemesListAsync
      (string InvoiceType, string Organisation, string SchemeType, string PaymentType);
}
