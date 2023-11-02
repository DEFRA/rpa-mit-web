namespace EST.MIT.Web.Repositories;

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
        var client = _clientFactory.CreateClient("ApprovalAPI");

        var response = await client.GetAsync($"/approvals/invoiceapprovers/{scheme}/{value}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> ValidateApprover(string approver)
    {
        var client = _clientFactory.CreateClient("ApprovalAPI");

        var body = new ValidateBody
        {
            scheme = "BPS",
            approverEmailAddress = approver
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
        public string scheme { get; set; }
        public string approverEmailAddress { get; set; }
    }
}