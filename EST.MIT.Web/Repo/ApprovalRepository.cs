using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Repositories;
public class ApprovalRepository : IApprovalRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ApprovalRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetApproversAsync(string scheme, string value)
    {
        var client = _clientFactory.CreateClient("ApprovalAPI");

        var response = await client.GetAsync($"/approvals/invoiceapprovers/{scheme}/{value}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> ValidateApproverAsync(string approver, string approvalGroup)
    {
        var client = _clientFactory.CreateClient("ApprovalAPI");

        var body = new ValidateBody
        {
            ApproverEmailAddress = approver,
            ApprovalGroup = approvalGroup
        };

        var response = await client.PostAsJsonAsync("/approvals/approver/validate", body);

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

    private struct ValidateBody
    {
        public string ApproverEmailAddress { get; set; }
        public string ApprovalGroup { get; set; }
    }
}