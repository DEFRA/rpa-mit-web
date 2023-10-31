using System.Net;
using EST.MIT.Web.Entities;
using Repositories;
using System.Security.Cryptography;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EST.MIT.Web.Services;

public interface IInvoiceAPI
{
    Task<Invoice> FindInvoiceAsync(string id, string scheme);
    Task<ApiResponse<Invoice>> SaveInvoiceAsync(Invoice invoice);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest);
    Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest, InvoiceLine invoiceLine);
    Task<ApiResponse<Invoice>> DeletePaymentRequestAsync(Invoice invoice, string paymentRequestId);
    Task<IEnumerable<Invoice>> GetApprovalsAsync();
}

public class InvoiceAPI : IInvoiceAPI
{
    private readonly ILogger<InvoiceAPI> _logger;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceAPI(IInvoiceRepository invoiceRepository, ILogger<InvoiceAPI> logger)
    {
        _logger = logger;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Invoice> FindInvoiceAsync(string id, string scheme) => await GetInvoice(id, scheme);
    public async Task<ApiResponse<Invoice>> SaveInvoiceAsync(Invoice invoice) => await SaveInvoice(invoice);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice) => await UpdateInvoice(invoice);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest) => await UpdateInvoice(invoice, paymentRequest);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest, InvoiceLine invoiceLine) => await UpdateInvoice(invoice, paymentRequest, invoiceLine);
    public async Task<ApiResponse<Invoice>> DeletePaymentRequestAsync(Invoice invoice, string paymentRequestId) => await DeletePaymentRequest(invoice, paymentRequestId);
    public async Task<IEnumerable<Invoice>> GetApprovalsAsync() => await GetApprovals();

    private async Task<Invoice> GetInvoice(string id, string scheme)
    {
        var response = await _invoiceRepository.GetInvoiceAsync(id, scheme);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                return await response.Content.ReadFromJsonAsync<Invoice>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing API response");
                return null;
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        _logger.LogError("Unknown response from API");
        return null;
    }

    private async Task<ApiResponse<Invoice>> SaveInvoice(Invoice invoice)
    {
        var errors = new Dictionary<string, List<string>>();
        invoice.Update();
        var response = await _invoiceRepository.PostInvoiceAsync(invoice);
        _logger.LogInformation($"Invoice {invoice.Id}: Received code {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.Created)
        {
            return new ApiResponse<Invoice>(response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest) return await BadRequestResponse<Invoice>(invoice, response, invoice.Id.ToString());

        errors.Add(response.StatusCode.ToString(), new List<string> { $"Unexpected response from API: ({(int)response.StatusCode})" });
        return new ApiResponse<Invoice>(response.StatusCode, errors);

    }

    private async Task<ApiResponse<Invoice>> UpdateInvoice(Invoice invoice)
    {
        var errors = new Dictionary<string, List<string>>();
        invoice.Update();
        var response = await _invoiceRepository.PutInvoiceAsync(invoice);
        _logger.LogInformation($"Invoice {invoice.Id}: Received code {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new ApiResponse<Invoice>(response.StatusCode) { Data = invoice };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest) return await BadRequestResponse<Invoice>(invoice, response, invoice.Id.ToString());

        errors.Add(response.StatusCode.ToString(), new List<string> { $"Unexpected response from API: ({(int)response.StatusCode})" });
        return new ApiResponse<Invoice>(response.StatusCode, errors);
    }

    private async Task<ApiResponse<Invoice>> UpdateInvoice(Invoice invoice, PaymentRequest paymentRequest)
    {
        var errors = new Dictionary<string, List<string>>();

        if (string.IsNullOrEmpty(paymentRequest.PaymentRequestId))
        {
            paymentRequest.PaymentRequestId = IdGenerator(paymentRequest.AgreementNumber);
            invoice.PaymentRequests.Add(paymentRequest);
        }

        invoice.Update();
        var response = await _invoiceRepository.PutInvoiceAsync(invoice);
        _logger.LogInformation($"Invoice {invoice.Id}: Received code {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new ApiResponse<Invoice>(response.StatusCode) { Data = invoice };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest) return await BadRequestResponse<Invoice>(invoice, response, invoice.Id.ToString());

        errors.Add(response.StatusCode.ToString(), new List<string> { $"Unexpected response from API: ({(int)response.StatusCode})" });
        return new ApiResponse<Invoice>(response.StatusCode, errors);
    }

    private async Task<ApiResponse<Invoice>> UpdateInvoice(Invoice invoice, PaymentRequest paymentRequest, InvoiceLine invoiceLine)
    {
        var errors = new Dictionary<string, List<string>>();

        invoiceLine.Id = Guid.NewGuid();
        invoice.PaymentRequests
                .First(x => x.PaymentRequestId == paymentRequest.PaymentRequestId).InvoiceLines
                .Add(invoiceLine);
        invoice.Update();
        var response = await _invoiceRepository.PutInvoiceAsync(invoice);
        _logger.LogInformation($"Invoice {invoice.Id}: Received code {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new ApiResponse<Invoice>(response.StatusCode) { Data = invoice };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest) return await BadRequestResponse<Invoice>(invoice, response, invoice.Id.ToString());

        errors.Add(response.StatusCode.ToString(), new List<string> { $"Unexpected response from API: ({(int)response.StatusCode})" });
        return new ApiResponse<Invoice>(response.StatusCode, errors);
    }

    private async Task<ApiResponse<Invoice>> DeletePaymentRequest(Invoice invoice, string paymentRequestId)
    {
        var errors = new Dictionary<string, List<string>>();
        invoice.PaymentRequests.RemoveAll(x => x.PaymentRequestId == paymentRequestId);
        invoice.Update();
        var response = await _invoiceRepository.PutInvoiceAsync(invoice);
        _logger.LogInformation($"Invoice {invoice.Id}: Received code {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation($"Invoice {invoice.Id}: Payment request {paymentRequestId} deleted");
            return new ApiResponse<Invoice>(response.StatusCode)
            {
                Data = (Invoice)invoice
            };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest) return await BadRequestResponse<Invoice>(invoice, response, invoice.Id.ToString());

        errors.Add(response.StatusCode.ToString(), new List<string> { $"Unexpected response from API: ({(int)response.StatusCode})" });
        return new ApiResponse<Invoice>(response.StatusCode, errors);

    }

    private async Task<IEnumerable<Invoice>> GetApprovals()
    {
        var response = await _invoiceRepository.GetApprovalsAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                var invoices = await response.Content.ReadFromJsonAsync<IEnumerable<Invoice>>() ?? new List<Invoice>();
                return invoices.Where(x => x.Status == "approval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing API response");
                return null;
            }

        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        _logger.LogError("Unknown response from API");
        return null;

    }

    private async Task<ApiResponse<T>> BadRequestResponse<T>(T validatable, HttpResponseMessage response, string id) where T : Validatable
    {
        var errors = new Dictionary<string, List<string>>();
        var message = await response.Content.ReadAsStringAsync();
        var validationErrors = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(message);
        if (validationErrors is not null)
        {
            foreach (var error in validationErrors.Errors)
            {
                errors.Add(error.Key, error.Value.ToList());
            }
            if (errors.Count > 0)
            {
                validatable.AddErrors(errors);
            }
            _logger.LogError($"Invoice {id}: {errors}");
        }
        return new ApiResponse<T>(response.StatusCode)
        {
            Data = validatable
        };
    }

    private static string IdGenerator(string agreementNumber)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var rng = RandomNumberGenerator.Create();

        for (int i = 0; i < stringChars.Length; i++)
        {
            byte[] randomNumber = new byte[1];
            rng.GetBytes(randomNumber);
            stringChars[i] = chars[randomNumber[0] % chars.Length];
        }

        var id = $"{agreementNumber}_{new String(stringChars).ToUpper()}";
        return id;

    }

}


