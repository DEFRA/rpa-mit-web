using System.Net;
using EST.MIT.Web.Entities;
using System.Security.Cryptography;
using AutoMapper;
using EST.MIT.Web.DTOs;
using EST.MIT.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;
public class InvoiceAPI : IInvoiceAPI
{
    private readonly ILogger<InvoiceAPI> _logger;
    private readonly IMapper _autoMapper;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceAPI(IInvoiceRepository invoiceRepository, ILogger<InvoiceAPI> logger, IMapper autoMapper)
    {
        _logger = logger;
        _autoMapper = autoMapper;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Invoice> FindInvoiceAsync(string id, string scheme) => await GetInvoice(id, scheme);
    public async Task<ApiResponse<Invoice>> SaveInvoiceAsync(Invoice invoice) => await SaveInvoice(invoice);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice) => await UpdateInvoice(invoice);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest) => await UpdateInvoice(invoice, paymentRequest);
    public async Task<ApiResponse<Invoice>> UpdateInvoiceAsync(Invoice invoice, PaymentRequest paymentRequest, InvoiceLine invoiceLine) => await UpdateInvoice(invoice, paymentRequest, invoiceLine);
    public async Task<ApiResponse<Invoice>> DeletePaymentRequestAsync(Invoice invoice, string paymentRequestId) => await DeletePaymentRequest(invoice, paymentRequestId);

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
                var dto = await response.Content.ReadFromJsonAsync<PaymentRequestsBatchDTO>();

                return this._autoMapper.Map<Invoice>(dto);
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

        var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);

        var response = await _invoiceRepository.PostInvoiceAsync(dto);
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
        if (invoice is null)
        {
            return new ApiResponse<Invoice>(HttpStatusCode.BadRequest);
        }

        var errors = new Dictionary<string, List<string>>();
        invoice.Update();

        foreach (var paymentRequest in invoice.PaymentRequests)
        {
            paymentRequest.Value = paymentRequest.InvoiceLines.Sum(invoiceLine => invoiceLine.Value);
        }

        var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);

        var response = await _invoiceRepository.PutInvoiceAsync(dto);
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

        var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);

        var response = await _invoiceRepository.PutInvoiceAsync(dto);
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

        if (invoiceLine.Id == Guid.Empty)
        {
            invoiceLine.Id = Guid.NewGuid();
        }

        var updatedPaymentRequest = invoice.PaymentRequests.First(x => x.PaymentRequestId == paymentRequest.PaymentRequestId);
        updatedPaymentRequest.InvoiceLines = updatedPaymentRequest.InvoiceLines.Where(line => line.Id != invoiceLine.Id).ToList();
        updatedPaymentRequest.InvoiceLines.Add(invoiceLine);
        updatedPaymentRequest.Value = updatedPaymentRequest.InvoiceLines.Sum(line => line.Value);

        invoice.Update();

        var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);

        var response = await _invoiceRepository.PutInvoiceAsync(dto);
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

        var dto = this._autoMapper.Map<PaymentRequestsBatchDTO>(invoice);

        var response = await _invoiceRepository.PutInvoiceAsync(dto);
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

    public async Task<IEnumerable<Invoice>> GetAllApprovalInvoicesAsync()
    {
        var response = await _invoiceRepository.GetAllApprovalsAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentRequestsBatchDTO>>();
                return this._autoMapper.Map<IEnumerable<Invoice>>(dtos);
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
    public async Task<Invoice> GetApprovalInvoiceAsync(string id)
    {
        var response = await _invoiceRepository.GetApprovalAsync(id);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                var dto = await response.Content.ReadFromJsonAsync<PaymentRequestsBatchDTO>();

                return this._autoMapper.Map<Invoice>(dto);
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

    public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
    {
        var response = await _invoiceRepository.GetInvoicesAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<Invoice>>() ?? new List<Invoice>();
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
}