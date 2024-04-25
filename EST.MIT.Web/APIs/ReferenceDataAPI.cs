using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.APIs;

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
                var organisations = await response.Content.ReadFromJsonAsync<IEnumerable<Organisation>>();
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK) { Data = organisations };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing Organisations: {ex.Message}");
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<Organisation>()
                }; ;
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
            error.Add("BadRequest", new List<string>() { "Invalid request was sent to API" });
            return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add("InternalServerError", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<SchemeType>>> GetSchemeTypesAsync(string? InvoiceType = null, string? Organisation = null)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemeTypesListAsync(InvoiceType, Organisation);

        _logger.LogInformation($"Calling Reference Data API for Schemes - params {InvoiceType}, {Organisation}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.NoContent);
            }

            try
            {
                var schemeTypes = await response.Content.ReadFromJsonAsync<IEnumerable<SchemeType>>();
                return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.OK) { Data = schemeTypes };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing Scheme Types: {ex.Message}");
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<SchemeType>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add("BadRequest", new List<string>() { "Invalid request was sent to API" });
            return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add("InternalServerError", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<SchemeType>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? InvoiceType = null, string? Organisation = null, string? SchemeType = null)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetPaymentTypesListAsync(InvoiceType, Organisation, SchemeType);

        _logger.LogInformation($"Calling Reference Data API for Payment Types - params {InvoiceType}, {Organisation}, {SchemeType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.NoContent);
            }

            try
            {
                var paymentType = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentType>>();
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK) { Data = paymentType };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentType>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<MainAccount>>> GetAccountsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetAccountsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Account Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.NoContent);
            }

            try
            {
                var mainAccount = await response.Content.ReadFromJsonAsync<IEnumerable<MainAccount>>();
                return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.OK) { Data = mainAccount };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<MainAccount>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<DeliveryBody>>> GetDeliveryBodiesAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetDeliveryBodiesListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Delivery Bodies - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.NoContent);
            }

            try
            {
                var deliveryBody = await response.Content.ReadFromJsonAsync<IEnumerable<DeliveryBody>>();
                return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.OK) { Data = deliveryBody };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<DeliveryBody>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<DeliveryBody>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<FundCode>>> GetFundsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetFundsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Fund Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var fundCode = await response.Content.ReadFromJsonAsync<IEnumerable<FundCode>>();
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK) { Data = fundCode };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<FundCode>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<MarketingYear>>> GetMarketingYearsAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetMarketingYearsListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Marketing Years - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.NoContent);
            }

            try
            {
                var marketingYears = await response.Content.ReadFromJsonAsync<IEnumerable<MarketingYear>>();
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK) { Data = marketingYears };
            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<MarketingYear>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemesAsync(string InvoiceType, string Organisation, string SchemeType, string PaymentType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemesListAsync(InvoiceType, Organisation, SchemeType, PaymentType);

        _logger.LogInformation($"Calling Reference Data API for Scheme Codes - params {InvoiceType}, {Organisation}, {SchemeType}, {PaymentType}");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var schemeCodes = await response.Content.ReadFromJsonAsync<IEnumerable<SchemeCode>>();
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK) { Data = schemeCodes };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing Scheme Codes: {ex.Message}");
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.InternalServerError, error) { Data = new List<SchemeCode>() };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add("BadRequest", new List<string>() { "Invalid request was sent to API" });
            return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add("InternalServerError", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.InternalServerError, error);
    }

}