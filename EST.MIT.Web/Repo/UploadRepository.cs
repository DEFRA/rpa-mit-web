using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.Repositories;

public interface IUploadRepository
{
    Task<HttpResponseMessage> GetUploads();
}

public class UploadRepository : IUploadRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public UploadRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }


    [ExcludeFromCodeCoverage]
    public async Task<HttpResponseMessage> GetUploads()
    {
        var client = _clientFactory.CreateClient("InvoiceImporterAPI");


        var response = await client.GetAsync($"/Uploads/userid");

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