using EST.MIT.Web.DTOs;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public InvoiceRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme) => await GetInvoice(id, scheme);
    public async Task<HttpResponseMessage> PostInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto) => await PostInvoice(paymentRequestsBatchDto);
    public async Task<HttpResponseMessage> PutInvoiceAsync(PaymentRequestsBatchDTO paymentRequestsBatchDto) => await PutInvoice(paymentRequestsBatchDto);
    public async Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequestDTO paymentRequestDto) => await DeleteHeader(paymentRequestDto);

    private async Task<HttpResponseMessage> GetInvoice(string id, string scheme)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/{scheme}/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> PostInvoice(PaymentRequestsBatchDTO paymentRequestsBatchDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PostAsJsonAsync($"/invoice", paymentRequestsBatchDto);

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> PutInvoice(PaymentRequestsBatchDTO paymentRequestsBatchDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PutAsJsonAsync($"/invoice/{paymentRequestsBatchDto.Id}", paymentRequestsBatchDto);

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteHeader(PaymentRequestDTO paymentRequestDto)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.DeleteAsync($"/invoice/header/{paymentRequestDto.PaymentRequestId}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetApprovalAsync(string id)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/approvals/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetAllApprovalsAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient("InvoiceAPI");

            var response = await client.GetAsync($"/invoice/approvals");

            await HandleHttpResponseError(response);

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async static Task HandleHttpResponseError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }
    }

    public async Task<HttpResponseMessage> GetInvoicesAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient("InvoiceAPI");

            var response = await client.GetAsync($"/invoices/user/xxx");

            await HandleHttpResponseError(response);

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}