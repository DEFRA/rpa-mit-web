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
        var requesterNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.requesterApproved)
                                .WithEmailRecipient(invoice.ApprovalRequestedByEmail)
                                .WithData(new NotificationInvoiceApprove
                                {
                                    ApproverEmail = invoice.ApproverEmail,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/summary/{invoice.SchemeType}/{invoice.Id}/user-invoices",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        var approverNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approverApproved)
                                .WithEmailRecipient(invoice.ApproverEmail)
                                .WithData(new NotificationInvoiceApprove
                                {
                                    ApproverEmail = invoice.ApproverEmail,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/summary/{invoice.SchemeType}/{invoice.Id}",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        try
        {
            var result = await UpdateAndNotifyAsync(InvoiceStatuses.Approved, invoice, requesterNotification, approverNotification);
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


    public async Task<bool> RejectInvoiceAsync(Invoice invoice, string reason)
    {
        var requesterNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.requesterRejected)
                                .WithEmailRecipient(invoice.ApprovalRequestedByEmail)
                                .WithData(new NotificationInvoiceReject
                                {
                                    Reason = reason,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/summary/{invoice.SchemeType}/{invoice.Id}/user-invoices",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        var approverNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approverRejected)
                                .WithEmailRecipient(invoice.ApproverEmail)
                                .WithData(new NotificationInvoiceReject
                                {
                                    Reason = reason,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/summary/{invoice.SchemeType}/{invoice.Id}",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        try
        {
            var result = await UpdateAndNotifyAsync(InvoiceStatuses.Rejected, invoice, requesterNotification, approverNotification);

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
        var requesterNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.requesterApproval)
                                .WithEmailRecipient(invoice.ApprovalRequestedByEmail)
                                .WithData(new NotificationOutstandingApproval
                                {
                                    Name = invoice.ApproverEmail,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/summary/{invoice.SchemeType}/{invoice.Id}/user-invoices",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        var approverNotification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approverApproval)
                                .WithEmailRecipient(invoice.ApproverEmail)
                                .WithData(new NotificationOutstandingApproval
                                {
                                    Name = invoice.ApproverEmail,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/user-approvals",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType,
                                })
                            .Build();

        try
        {
            var response = await UpdateAndNotifyAsync(InvoiceStatuses.AwaitingApproval, invoice, requesterNotification, approverNotification);
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
    public async Task<ApiResponse<Invoice>> UpdateAndNotifyAsync(string status, Invoice invoice, Notification requesterNotification, Notification approverNotification)
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

            var addedToRequesterQueue = await _eventQueueService.AddMessageToQueueAsync("invoicenotification", requesterNotification.ToMessage());
            if (!addedToRequesterQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to queue");
                response.Errors.Add("NotificationQueue", new List<string> { "Failed to add to queue" });
                response.IsSuccess = false;
                return response;
            }
            _logger.LogInformation($"Invoice {invoice.Id}: Added to queue");

            var addedToApproverQueue = await _eventQueueService.AddMessageToQueueAsync("invoicenotification", approverNotification.ToMessage());
            if (!addedToApproverQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to queue");
                response.Errors.Add("NotificationQueue", new List<string> { "Failed to add to queue" });
                response.IsSuccess = false;
                return response;
            }
            _logger.LogInformation($"Invoice {invoice.Id}: Added to queue");

            var addedToRequesterNotificationQueue = await _notificationQueueService.AddMessageToQueueAsync(requesterNotification);
            if (!addedToRequesterNotificationQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to notification queue");
                response.Errors.Add("NotificationQueue", new List<string> { "Failed to add to notification queue" });
                response.IsSuccess = false;
                return response;
            }

            var addedToApproverNotificationQueue = await _notificationQueueService.AddMessageToQueueAsync(approverNotification);
            if (!addedToApproverNotificationQueue)
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