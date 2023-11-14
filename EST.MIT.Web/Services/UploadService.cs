using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class UploadService : IUploadService
{
    private readonly IAzureBlobService _blobService;
    private readonly IEventQueueService _eventQueueService;
    private readonly IImporterQueueService _importerQueueService;
    private readonly ILogger<UploadService> _logger;

    public UploadService(ILogger<UploadService> logger, IAzureBlobService blobService, IEventQueueService eventQueueService, IImporterQueueService importerQueueService)
    {
        _logger = logger;
        _blobService = blobService;
        _eventQueueService = eventQueueService;
        _importerQueueService = importerQueueService;
    }

    public async Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, string schemeType, string organisation, string paymentType, string accountType, string createdBy)
    {

        var importRequest = new ImportRequest()
        {
            FileName =  file.Name,
            FileSize = file.Size,
            FileType = file.Name?.Split('.').Last() ?? string.Empty,
            Timestamp = DateTimeOffset.Now,
            AccountType = accountType,
            SchemeType = schemeType,
            Organisation = organisation,
            PaymentType = paymentType,
            CreatedBy = createdBy,
            BlobFileName = Path.GetRandomFileName().Split('.').First(),
            BlobFolder = "import"
        };

        var importRequestSummary = new ImportRequestSummary(importRequest, GenerateConfirmationNumber());

        try
        {
            await _blobService.AddFileToBlobAsync(BlobPath(importRequest), file);
            await _importerQueueService.AddMessageToQueueAsync(importRequest);
            await _eventQueueService.AddMessageToQueueAsync("invoice-importer", importRequestSummary.ToMessage());
            // TODO: implement bulk upload confirmation await _invoiceRepository.SaveBulkUploadConfirmation(summary);
            await _eventQueueService.AddMessageToQueueAsync("confirmation-notification-queue", importRequestSummary.ToMessage());

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(importRequestSummary.ConfirmationNumber)
            };

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occoured. Error: {message}", e.Message);
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }

    public static string BlobPath(ImportRequest importRequest) => $"{importRequest.BlobFolder}/{importRequest.BlobFileName}";
    public static string GenerateConfirmationNumber() => $"BLK-{Guid.NewGuid().ToString().Substring(0, 8)}";

    //private static UploadFileSummary CreateUploadSummaryMessage(IBrowserFile file, string schemeType, string organisation, string paymentType, string accountType, string createdBy)
    //{
    //    return new UploadFileSummary(GenerateConfirmationNumber())
    //    {
    //        FileName = Path.GetRandomFileName().Split('.').First(),
    //        FileSize = file.Size,
    //        FileType = file.Name?.Split('.').Last() ?? string.Empty,
    //        Timestamp = DateTimeOffset.Now,
    //        AccountType = accountType,
    //        SchemeType = schemeType,
    //        Organisation = organisation,
    //        PaymentType = paymentType,
    //        CreatedBy = createdBy
    //    };
    //}
}