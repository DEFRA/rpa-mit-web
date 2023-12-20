using System.Net;
using EST.MIT.Web.Builders;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;

namespace EST.MIT.Web.Services;

public class ApprovalService : IApprovalService
{
    private readonly IEventQueueService _eventQueueService;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly IInvoiceAPI _invoiceAPI;
    private readonly IApprovalAPI _approvalAPI;
    private readonly ILogger<ApprovalService> _logger;
    private readonly IHttpContextAccessor _context;

    public ApprovalService(IEventQueueService queueService, INotificationQueueService notificationQueueService,
        IInvoiceAPI invoiceAPI, IApprovalAPI approvalAPI, ILogger<ApprovalService> logger, IHttpContextAccessor context)
    {
        _eventQueueService = queueService;
        _notificationQueueService = notificationQueueService;
        _invoiceAPI = invoiceAPI;
        _approvalAPI = approvalAPI;
        _logger = logger;
        _context = context;
    }
    public async Task<Invoice> GetInvoiceAsync(string id, string scheme) => await _invoiceAPI.FindInvoiceAsync(id, scheme);

    public async Task<bool> ApproveInvoiceAsync(Invoice invoice)
    {
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approved)
                                .WithData(new NotificationInvoiceApprove
                                {
                                    ApproverEmail = "user"
                                })
                            .Build();

        try
        {
            var result = await UpdateAndNotifyAsync(InvoiceStatuses.Approved, invoice, notification);
            if (!result.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Invoice {invoice.Id}: Error during approval process");
            return false;
        }
    }


    public async Task<bool> RejectInvoiceAsync(Invoice invoice, string justification)
    {
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.rejected)
                                .WithData(new NotificationInvoiceReject
                                {
                                    Justification = justification,
                                    Approver = "user"
                                })
                            .Build();
        try
        {
            var result = await UpdateAndNotifyAsync(InvoiceStatuses.Rejected, invoice, notification);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Invoice {invoice.Id}: Error during rejection process");
            return false;
        }
    }

    public async Task<ApiResponse<Invoice>> SubmitApprovalAsync(Invoice invoice)
    {
        var errors = new Dictionary<string, List<string>>();
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approval)
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

        try
        {
            var response = await UpdateAndNotifyAsync(InvoiceStatuses.AwaitingApproval, invoice, notification);
            if (!response.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Submission failed");
                errors = response.Errors;
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Invoice {invoice.Id}: Error during submission process");
            return new ApiResponse<Invoice>(HttpStatusCode.InternalServerError)
            {
                Errors = errors
            };
        }
    }

    public async Task<ApiResponse<Invoice>> UpdateAndNotifyAsync(string status, Invoice invoice, Notification notification)
    {
        try
        {
            invoice.Update(status);

            var response = await _invoiceAPI.UpdateInvoiceAsync(invoice);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to update invoice");
                foreach (var error in response.Errors)
                {
                    _logger.LogError($"Invoice {invoice.Id}: {error.Key} - {error.Value}");
                }
                return response;

            }
            _logger.LogInformation($"Invoice {invoice.Id}: Updated");

            var addedToQueue = await _eventQueueService.AddMessageToQueueAsync("invoicenotification", notification.ToMessage());
            if (!addedToQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to queue");
                response.Errors.Add("NotificationQueue", new List<string> { "Failed to add to queue" });
                response.IsSuccess = false;
                return response;
            }
            _logger.LogInformation($"Invoice {invoice.Id}: Added to queue");

            var addedToNotificationQueue = await _notificationQueueService.AddMessageToQueueAsync(notification);
            if (!addedToNotificationQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to notification queue");
                response.Errors.Add("NotificationQueue", new List<string> { "Failed to add to notification queue" });
                response.IsSuccess = false;
                return response;
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invoice {invoice.Id}: {ex.Message}");
            return new ApiResponse<Invoice>(false) { Errors = new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } } };
        }
    }

    public async Task<Dictionary<string, string>> GetApproversAsync(string scheme, string value)
    {
        var approvers = new Dictionary<string, string>();
        var response = await _approvalAPI.GetApproversAsync(scheme, value);

        if (response.IsSuccess)
        {
            return (Dictionary<string, string>)response.Data;
        }
        return approvers;
    }

    public async Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver, string approvalGroup) => await _approvalAPI.ValidateApproverAsync(approver, approvalGroup);

}