using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using Entities;

namespace Services;

public interface IUploadService
{
    Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file);
}

public class UploadService : IUploadService
{
    private readonly IBlobService _blobService;
    private readonly IQueueService _queueService;
    private readonly ILogger<UploadService> _logger;

    public UploadService(ILogger<UploadService> logger, IBlobService blobService, IQueueService queueService)
    {
        _logger = logger;
        _blobService = blobService;
        _queueService = queueService;
    }

    public async Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file)
    {
        var summary = GenerateMessage(file);

        try
        {
            await _blobService.AddFileToBlobAsync(summary.FileName, "invoices", file, "import");
            await _queueService.AddMessageToQueueAsync("invoice-importer", summary.ToMessage());
            // TODO: implement bulk upload confirmation await _invoiceRepository.SaveBulkUploadConfirmation(summary);
            await _queueService.AddMessageToQueueAsync("confirmation-notification-queue", summary.ToMessage());

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(summary.ConfirmationNumber)
            };

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occoured. Error: {message}", e.Message);
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }

    public static string GenerateConfirmationNumber() => $"BLK-{Guid.NewGuid().ToString().Substring(0, 8)}";

    private static UploadFileSummary GenerateMessage(IBrowserFile file)
    {
        return new UploadFileSummary(GenerateConfirmationNumber())
        {
            FileName = Path.GetRandomFileName().Split('.').First(),
            FileSize = file.Size,
            FileType = file.Name?.Split('.').Last() ?? string.Empty,
            Timestamp = DateTimeOffset.Now
        };
    }

}