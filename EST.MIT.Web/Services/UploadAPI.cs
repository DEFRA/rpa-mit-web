using System.Net;
using EST.MIT.Web.Entities;
using Repositories;

namespace EST.MIT.Web.Services;

public interface IUploadAPI
{
    Task<IEnumerable<ImportRequest>> GetUploadsAsync();
}

public class UploadAPI : IUploadAPI
{
    private readonly ILogger<UploadAPI> _logger;
    private readonly IUploadRepository _uploadRepository;

    public UploadAPI(IUploadRepository uploadRepository, ILogger<UploadAPI> logger)
    {
        _logger = logger;
        _uploadRepository = uploadRepository;
    }

    public async Task<IEnumerable<ImportRequest>> GetUploadsAsync()
    {
        var response = await _uploadRepository.GetUploads();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ImportRequest>>();
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