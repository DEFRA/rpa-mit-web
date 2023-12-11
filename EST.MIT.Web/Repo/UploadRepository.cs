using System.Net;
using System.Text.Json;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Repositories;

public class UploadRepository : IUploadRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public UploadRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }


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
    public async Task<HttpResponseMessage> GetFileByImportRequestIdAsync(Guid importRequestId)
    {
        var client = _clientFactory.CreateClient("InvoiceImporterAPI");

        var response = await client.GetAsync($"/UploadedFile/{importRequestId}");

        await HandleHttpResponseError(response);

        return response;
    }
}