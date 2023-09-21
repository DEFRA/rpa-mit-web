using System.Diagnostics.CodeAnalysis;
using System.Net;
using Entities;
using System.Text.Json;
using AutoMapper;
using EST.MIT.Web.DTOs;

namespace Repositories;

public interface IInvoiceRepository
{
    Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme);
    Task<HttpResponseMessage> PostInvoiceAsync(Invoice invoice);
    Task<HttpResponseMessage> PutInvoiceAsync(Invoice invoice);
    Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequest paymentRequest);
    Task<HttpResponseMessage> GetApprovalAsync(string id, string scheme);
    Task<HttpResponseMessage> GetApprovalsAsync();
}

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMapper _autoMapper;

    public InvoiceRepository(IHttpClientFactory clientFactory, IMapper autoMapper)
    {
        _clientFactory = clientFactory;
        _autoMapper = autoMapper;
    }

    public async Task<HttpResponseMessage> GetInvoiceAsync(string id, string scheme) => await GetInvoice(id, scheme);
    public async Task<HttpResponseMessage> PostInvoiceAsync(Invoice invoice) => await PostInvoice(invoice);
    public async Task<HttpResponseMessage> PutInvoiceAsync(Invoice invoice) => await PutInvoice(invoice);
    public async Task<HttpResponseMessage> DeleteHeaderAsync(PaymentRequest paymentRequest) => await DeleteHeader(paymentRequest);
    public async Task<HttpResponseMessage> GetApprovalAsync(string id, string scheme) => await GetApproval(id, scheme);
    public async Task<HttpResponseMessage> GetApprovalsAsync() => await GetApprovals();


    private async Task<HttpResponseMessage> GetInvoice(string id, string scheme)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/{scheme}/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    private async Task<HttpResponseMessage> PostInvoice(Invoice invoice)
    {
        // Do the mapping from the entity to the DTO here
        // var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);
        // and then post the DTO instead of the entity

        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PostAsJsonAsync($"/invoice", invoice);

        await HandleHttpResponseError(response);

        return response;
    }


    private async Task<HttpResponseMessage> PutInvoice(Invoice invoice)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.PutAsJsonAsync($"/invoice/{invoice.Id}", invoice);

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteHeader(PaymentRequest paymentRequest)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.DeleteAsync($"/invoice/header/{paymentRequest.PaymentRequestId}");

        await HandleHttpResponseError(response);

        return response;
    }

    public async Task<HttpResponseMessage> GetApproval(string id, string scheme)
    {
        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = await client.GetAsync($"/invoice/approval/{scheme}/{id}");

        await HandleHttpResponseError(response);

        return response;
    }

    [ExcludeFromCodeCoverageAttribute]
    public async Task<HttpResponseMessage> GetApprovals()
    {

        //placeholder until the API is ready

        var client = _clientFactory.CreateClient("InvoiceAPI");

        var response = new HttpResponseMessage();

        var invoices = new List<Invoice>();
        invoices.Add(new Invoice
        {
            SchemeType = "scheme",
            Approver = "approver",
            PaymentType = "invoice",
            AccountType = "account",
            Organisation = "organisation",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    CustomerId = 1234567890,
                    Value = 420
                }
            }
        });

        invoices.Add(new Invoice
        {
            SchemeType = "scheme",
            Approver = "approver",
            PaymentType = "invoice",
            AccountType = "account",
            Organisation = "organisation",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    CustomerId = 1122334455,
                    Value = 6969
                }
            }
        });


        response.Content = new StringContent(JsonSerializer.Serialize(invoices));
        response.StatusCode = HttpStatusCode.OK;

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