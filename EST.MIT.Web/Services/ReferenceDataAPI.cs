using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class ReferenceDataAPI : IReferenceDataAPI
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ILogger<ReferenceDataAPI> _logger;

    public ReferenceDataAPI(IReferenceDataRepository referenceDataRepository, ILogger<ReferenceDataAPI> logger)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string InvoiceType)
        => await GetOrganisations(InvoiceType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? InvoiceType, string? Organisation)
        => await GetSchemeTypes(InvoiceType, Organisation);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetPaymentTypesAsync(string? InvoiceType, string? Organisation, string? SchemeType)
       => await GetPaymentTypes(InvoiceType, Organisation, SchemeType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetAccountsAsync(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
       => await GetAccounts(InvoiceType, Organisation, SchemeType, PaymentType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetDeliveryBodiesAsync(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
       => await GetDeliveryBodies(InvoiceType, Organisation, SchemeType, PaymentType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetFundsAsync(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
       => await GetFunds(InvoiceType, Organisation, SchemeType, PaymentType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetMarketingYearsAsync(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
       => await GetMarketingYears(InvoiceType, Organisation, SchemeType, PaymentType);
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
       => await GetSchemes(InvoiceType, Organisation, SchemeType, PaymentType);

    private async Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisations(string InvoiceType)
    {
        var error = new Dictionary<string, List<string>>();

        _logger.LogInformation($"Calling Reference Data API for Organisations - params {InvoiceType}");
        var response = await _referenceDataRepository.GetOrganisationsListAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<Organisation>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<Organisation>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });
            return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypes(string? InvoiceType, string? Organisation)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemeTypesListAsync(InvoiceType, Organisation);

        _logger.LogInformation($"Calling Reference Data API for Schemes - params {InvoiceType}, {Organisation}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetPaymentTypes(string? InvoiceType, string? Organisation, string? SchemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetPaymentTypesListAsync(InvoiceType, Organisation, SchemeType);

        _logger.LogInformation($"Calling Reference Data API for Payment Types - params {InvoiceType}, {Organisation}, {SchemeType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetAccounts(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetAccountsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Account Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetDeliveryBodies(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetDeliveryBodiesListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Delivery Bodies - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetFunds(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetFundsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Fund Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetMarketingYears(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetMarketingYearsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Marketing Years - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemes(string? InvoiceType, string? Organisation, string? SchemeType, string? PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemesListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Scheme Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                {
                    Data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            _logger.LogError(x.Exception?.Message);
                            throw new Exception(x.Exception?.Message);
                        }
                        return x.Result;
                    })
                };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }
}