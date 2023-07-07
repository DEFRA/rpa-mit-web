using System.Net;
using Entities;
using Repositories;

namespace Services;

public interface IReferenceDataAPI
{
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string InvoiceType);
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync(string InvoiceType, string Organisation);
}

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
    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync(string InvoiceType, string Organisation)
        => await GetSchemes(InvoiceType, Organisation);

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

    private async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemes(string InvoiceType, string Organisation)
    {
        var error = new Dictionary<string, List<string>>();

        _logger.LogInformation($"Calling Reference Data API for Schemes - params {InvoiceType}, {Organisation}");
        var response = await _referenceDataRepository.GetSchemesListAsync(InvoiceType, Organisation);

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