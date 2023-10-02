namespace Repositories;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetOrganisationsListAsync();
    Task<HttpResponseMessage> GetSchemesListAsync(string? InvoiceType = null, string? Organisation = null);
    Task<HttpResponseMessage> GetPaymentTypesListAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null);
    Task<HttpResponseMessage> GetFundListAsync
        (string InvoiceType, string Organisation, string Scheme, string PaymentType);
}

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetOrganisationsListAsync()
        => await GetOrganisationsList();
    public async Task<HttpResponseMessage> GetSchemesListAsync(string? InvoiceType, string? Organisation)
        => await GetSchemesList(InvoiceType, Organisation);
    public async Task<HttpResponseMessage> GetPaymentTypesListAsync(string? InvoiceType, string? Organisation, string? SchemeType)
        => await GetPaymentTypesList(InvoiceType, Organisation, SchemeType);
    public async Task<HttpResponseMessage> GetFundListAsync
        (string InvoiceType, string Organisation, string Scheme, string PaymentType)
        => await GetFundsList(InvoiceType, Organisation, Scheme, PaymentType);

    private async Task<HttpResponseMessage> GetOrganisationsList()
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/organisations");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> GetSchemesList(string? InvoiceType, string? Organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = (string.IsNullOrEmpty(InvoiceType) && string.IsNullOrEmpty(Organisation))
                    ? await client.GetAsync($"/schemeTypes")
                    : await client.GetAsync($"/schemeTypes?invoiceType={InvoiceType}&organisation={Organisation}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> GetPaymentTypesList(string? InvoiceType, string? Organisation, string? SchemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = (string.IsNullOrEmpty(InvoiceType) && string.IsNullOrEmpty(Organisation))
                    ? await client.GetAsync($"/paymentTypes")
                    : await client.GetAsync($"/paymentTypes?invoiceType={InvoiceType}&organisation={Organisation}&schemeType={SchemeType}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> GetFundsList
        (string InvoiceType, string Organisation, string Scheme, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/funds/{InvoiceType}/{Organisation}/{Scheme}/{PaymentType}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async static Task HandleHttpResponseError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }
    }

}