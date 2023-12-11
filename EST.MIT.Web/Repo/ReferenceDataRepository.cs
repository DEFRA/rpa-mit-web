using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetOrganisationsListAsync()
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/organisations");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetSchemeTypesListAsync(string? InvoiceType, string? Organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = (string.IsNullOrEmpty(InvoiceType) && string.IsNullOrEmpty(Organisation))
                    ? await client.GetAsync($"/schemeTypes")
                    : await client.GetAsync($"/schemeTypes?invoiceType={InvoiceType}&organisation={Organisation}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetPaymentTypesListAsync(string? InvoiceType, string? Organisation, string? SchemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = (string.IsNullOrEmpty(InvoiceType) && string.IsNullOrEmpty(Organisation))
                    ? await client.GetAsync($"/paymentTypes")
                    : await client.GetAsync($"/paymentTypes?invoiceType={InvoiceType}&organisation={Organisation}&schemeType={SchemeType}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetAccountsListAsync
       (string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/accounts/{InvoiceType}/{Organisation}/{SchemeType}/{PaymentType}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetDeliveryBodiesListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/deliveryBodies/{InvoiceType}/{Organisation}/{SchemeType}/{PaymentType}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetFundsListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/funds/{InvoiceType}/{Organisation}/{SchemeType}/{PaymentType}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetMarketingYearsListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/marketingYears/{InvoiceType}/{Organisation}/{SchemeType}/{PaymentType}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetSchemesListAsync
        (string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataAPI");

        var response = await client.GetAsync($"/schemes/{InvoiceType}/{Organisation}/{SchemeType}/{PaymentType}");

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