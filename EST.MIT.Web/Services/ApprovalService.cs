using EST.MIT.Web.Builders;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class ApprovalService : IApprovalService
{
    private readonly IEventQueueService _eventQueueService;
    private readonly IInvoiceAPI _invoiceAPI;
    private readonly IApprovalAPI _approvalAPI;
    private readonly ILogger<ApprovalService> _logger;
    private readonly IHttpContextAccessor _context;

    public ApprovalService(IEventQueueService queueService, IInvoiceAPI invoiceAPI, IApprovalAPI approvalAPI, ILogger<ApprovalService> logger, IHttpContextAccessor context)
    {
        _eventQueueService = queueService;
        _invoiceAPI = invoiceAPI;
        _approvalAPI = approvalAPI;
        _logger = logger;
        _context = context;
    }

    public async Task<Invoice> GetInvoiceAsync(string id, string scheme) => await _invoiceAPI.FindInvoiceAsync(id, scheme);
    public async Task<bool> ApproveInvoiceAsync(Invoice invoice) => await ApproveInvoice(invoice);
    public async Task<bool> RejectInvoiceAsync(Invoice invoice, string justification) => await RejectInvoice(invoice, justification);
    public async Task<ApiResponse<Invoice>> SubmitApprovalAsync(Invoice invoice) => await SubmitApproval(invoice);
    public async Task<Dictionary<string, string>> GetApproversAsync(string scheme, string value) => await GetApprovers();
    //public async Task<Invoice> GetApprovalAsync(string id, string scheme) => await GetApproval(id, scheme);
   // public async Task<IEnumerable<Invoice>> GetOutstandingApprovalsAsync() => await GetOutstandingApprovals();
    public async Task<ApiResponse<BoolRef>> ValidateApproverAsync(string approver, string scheme) => await ValidateApprover(approver, scheme);

    private async Task<bool> ApproveInvoice(Invoice invoice)
    {
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approved)
                                .WithData(new NotificationInvoiceApprove
                                {
                                    Approver = "user"
                                })
                            .Build();

        return await UpdateAndNotify("approved", invoice, notification).ContinueWith(x =>
        {
            if (!x.Result.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;

        });
    }

    private async Task<bool> RejectInvoice(Invoice invoice, string justification)
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

        return await UpdateAndNotify("rejected", invoice, notification).ContinueWith(x =>
        {
            if (!x.Result.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;

        });
    }

    private async Task<ApiResponse<Invoice>> SubmitApproval(Invoice invoice)
    {
        var errors = new Dictionary<string, List<string>>();
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approval)
                                .WithData(new NotificationOutstandingApproval
                                {
                                    Name = invoice.Approver,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/details/{invoice.SchemeType}/{invoice.Id}/true",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType
                                })
                            .Build();

        return await UpdateAndNotify("approval", invoice, notification).ContinueWith(x =>
        {
            if (!x.Result.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Submission failed");
                errors = x.Result.Errors;
                return x.Result;
            }
            return x.Result;
        });

    }

    private async Task<ApiResponse<Invoice>> UpdateAndNotify(string status, Invoice invoice, Notification notification)
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
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invoice {invoice.Id}: {ex.Message}");
            return new ApiResponse<Invoice>(false) { Errors = new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } } };
        }
    }
    
    private async Task<Dictionary<string, string>> GetApprovers()
    {
        var approvers = new Dictionary<string, string>();
        var response = await _approvalAPI.GetApproversAsync("scheme", "value");

        if (response.IsSuccess)
        {
            return (Dictionary<string, string>)response.Data;
        }
        return approvers;
    }

    //private async Task<IEnumerable<Invoice>> GetOutstandingApprovals()
    //{
    //    var invoices = await _invoiceAPI.GetApprovalsAsync();
    //    return invoices;
    //}

    private async Task<ApiResponse<BoolRef>> ValidateApprover(string approver, string scheme) => await _approvalAPI.ValidateApproverAsync(approver, scheme);

}