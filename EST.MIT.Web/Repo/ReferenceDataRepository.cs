namespace Repositories;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetOrganisationsListAsync();
    Task<HttpResponseMessage> GetSchemesListAsync(string InvoiceType, string Organisation);
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
    public async Task<HttpResponseMessage> GetSchemesListAsync(string InvoiceType, string Organisation)
        => await GetSchemesList(InvoiceType, Organisation);
    public async Task<HttpResponseMessage> GetFundListAsync
        (string InvoiceType, string Organisation, string Scheme, string PaymentType)
        => await GetFundsList(InvoiceType, Organisation, Scheme, PaymentType);

    private async Task<HttpResponseMessage> GetOrganisationsList()
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/organisations?invoiceType=AP");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> GetSchemesList(string InvoiceType, string Organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response =
            await client.GetAsync($"/schemeTypes?invoiceType={InvoiceType}&organisation={Organisation}");

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