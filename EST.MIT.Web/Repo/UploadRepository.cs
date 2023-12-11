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
        return await Task.Run(() =>
        {
            var mockImportRequests = new List<ImportRequest>
            {
            new ImportRequest
            {
                ImportRequestId = Guid.NewGuid(),
                FileName = "mockfile1.xlsx",
                FileSize = 2048,
                FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Timestamp = DateTimeOffset.Now,
                PaymentType = "MockType1",
                Organisation = "MockOrg1",
                SchemeType = "MockScheme1",
                AccountType = "MockAccount1",
                CreatedBy = "mockuser1@example.com",
                Status = UploadStatus.Uploaded,
                BlobFileName = "MockBlobFileName1",
                BlobFolder = "MockBlobFolder1"
            },
                // Add more mock ImportRequest objects as needed
            };

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockImportRequests))
            };

            return mockResponse;
        });
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