using System.Net;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Repositories;

namespace EST.MIT.Web.Services;

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
        _logger.LogError("Getting Uploads from API");

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

    public async Task<byte[]> GetFileByImportRequestIdAsync(Guid importRequestId)
    {
        _logger.LogError("Getting File by Id from API");

        var response = await _uploadRepository.GetFileByImportRequestIdAsync(importRequestId);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"File not found: {importRequestId}");
            return null;
        }
        else if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("API returned no data");
                return null;
            }

            try
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing API response");
                return null;
            }
        }
        else
        {
            _logger.LogError($"Unknown response from API: {response.StatusCode}");
            return null;
        }
    }


    //public async Task<byte[]> GetFileByFileNameAsync(string fileName)
    //{
    //    _logger.LogError("Getting File by Filename from API");

    //    var response = await _uploadRepository.GetFileByFileNameAsync(fileName);

    //    if (response.StatusCode == HttpStatusCode.NotFound)
    //    {
    //        _logger.LogWarning($"File not found: {fileName}");
    //        return null;
    //    }
    //    else if (response.StatusCode == HttpStatusCode.OK)
    //    {
    //        if (response.Content.Headers.ContentLength == 0)
    //        {
    //            _logger.LogWarning("API returned no data");
    //            return null;
    //        }

    //        try
    //        {
    //            return await response.Content.ReadAsByteArrayAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error deserializing API response");
    //            return null;
    //        }
    //    }
    //    else
    //    {
    //        _logger.LogError($"Unknown response from API: {response.StatusCode}");
    //        return null;
    //    }
    //}

}