using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Builders;
using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Services;

public class UploadService : IUploadService
{
    private readonly IAzureBlobService _blobService;
    private readonly IEventQueueService _eventQueueService;
    private readonly IImporterQueueService _importerQueueService;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly ILogger<UploadService> _logger;
    private readonly IHttpContextAccessor _context;

    public UploadService(ILogger<UploadService> logger, 
        IAzureBlobService blobService, 
        IEventQueueService eventQueueService, 
        IImporterQueueService importerQueueService,
        INotificationQueueService notificationQueueService,
        IHttpContextAccessor context)
    {
        _logger = logger;
        _blobService = blobService;
        _eventQueueService = eventQueueService;
        _importerQueueService = importerQueueService;
        _notificationQueueService = notificationQueueService;
        _context = context;
    }

    public async Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, Invoice invoice)
    {
        var response = new HttpResponseMessage();
        var status = NotificationType.approval;

        var importRequest = new ImportRequest()
        {
            FileName = file.Name,
            FileSize = file.Size,
            FileType = file.Name?.Split('.').Last() ?? string.Empty,
            Timestamp = DateTimeOffset.Now,
            AccountType = invoice.AccountType,
            SchemeType = invoice.SchemeType,
            Organisation = invoice.Organisation,
            PaymentType = invoice.PaymentType,
            CreatedBy = invoice.CreatedBy,
            BlobFileName = Path.GetRandomFileName().Split('.')[0],
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

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(importRequestSummary.ConfirmationNumber);
                                       
            return response;

        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error occoured. Error: {message}", e.Message);
            response.StatusCode = HttpStatusCode.InternalServerError;
            status = NotificationType.rejected;
        }

        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(status)
                                .WithEmailRecipient(invoice.ApproverEmail)
                                .WithData(new NotificationOutstandingApproval
                                {
                                    Name = invoice.ApproverEmail,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/details/{invoice.SchemeType}/{invoice.Id}/true",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType
                                })
                            .Build();
        var addedToNotificationQueue = await _notificationQueueService.AddMessageToQueueAsync(notification);
        if (!addedToNotificationQueue)
        {
            _logger.LogError($"Invoice {invoice.Id}: Failed to add to notification queue");
            response.StatusCode = HttpStatusCode.InternalServerError;            
        }

        return response;
    }

    public static string BlobPath(ImportRequest importRequest) => $"{importRequest.BlobFolder}/{importRequest.BlobFileName}";
    public static string GenerateConfirmationNumber() => $"BLK-{Guid.NewGuid().ToString().Substring(0, 8)}";
}