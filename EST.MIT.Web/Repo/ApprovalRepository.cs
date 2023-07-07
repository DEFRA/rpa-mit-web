using System.Net;

namespace Repositories;

public interface IApprovalRepository
{
    Task<HttpResponseMessage> GetApproversAsync(string scheme, string value);
    Task<HttpResponseMessage> ValidateApproverAsync(string approver);
}

public class ApprovalRepository : IApprovalRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ApprovalRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetApproversAsync(string scheme, string value)
        => await GetApprovers(scheme, value);
    public async Task<HttpResponseMessage> ValidateApproverAsync(string approver) => await ValidateApprover(approver);

    private async Task<HttpResponseMessage> GetApprovers(string scheme, string value)
    {
        var client = _clientFactory.CreateClient("ApproversAPI");

        var response = await client.GetAsync($"/approvals/invoiceapprovers/{scheme}/{value}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> ValidateApprover(string approver)
    {
        var client = _clientFactory.CreateClient("ApproversAPI");

        var response = await client.GetAsync($"/approvals/approver/validate/{approver}");

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