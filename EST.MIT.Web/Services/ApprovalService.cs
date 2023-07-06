using Builders;
using Entities;
using Helpers;

namespace Services;

public interface IApprovalService
{
    Task<Invoice> GetInvoiceAsync(string id, string scheme);
    Task<bool> ApproveInvoiceAsync(Invoice invoice);
    Task<bool> RejectInvoiceAsync(Invoice invoice, string justification);
    Task<bool> SubmitApprovalAsync(Invoice invoice);
    Task<Dictionary<string, string>> GetApproversAsync(string scheme, string value);
    Task<Invoice> GetApprovalAsync(string id, string scheme);
    Task<IEnumerable<Invoice>> GetOutstandingApprovalsAsync();
}

public class ApprovalService : IApprovalService
{
    private readonly IQueueService _queueService;
    private readonly IInvoiceAPI _invoiceAPI;
    private readonly IApprovalAPI _approvalAPI;
    private readonly ILogger<ApprovalService> _logger;
    private readonly IHttpContextAccessor _context;

    public ApprovalService(IQueueService queueService, IInvoiceAPI invoiceAPI, IApprovalAPI approvalAPI, ILogger<ApprovalService> logger, IHttpContextAccessor context)
    {
        _queueService = queueService;
        _invoiceAPI = invoiceAPI;
        _approvalAPI = approvalAPI;
        _logger = logger;
        _context = context;
    }

    public async Task<Invoice> GetInvoiceAsync(string id, string scheme) => await _invoiceAPI.FindInvoiceAsync(id, scheme);
    public async Task<bool> ApproveInvoiceAsync(Invoice invoice) => await ApproveInvoiceImplementationAsync(invoice);
    public async Task<bool> RejectInvoiceAsync(Invoice invoice, string justification) => await RejectInvoiceImplementationAsync(invoice, justification);
    public async Task<bool> SubmitApprovalAsync(Invoice invoice) => await SubmitApprovalImplementationAsync(invoice);
    public async Task<Dictionary<string, string>> GetApproversAsync(string scheme, string value) => await GetApproversImplementationAsync();
    public async Task<Invoice> GetApprovalAsync(string id, string scheme) => await GetApprovalImplementationAsync(id, scheme);
    public async Task<IEnumerable<Invoice>> GetOutstandingApprovalsAsync() => await GetOutstandingApprovalsImplementationAsync();

    private async Task<bool> ApproveInvoiceImplementationAsync(Invoice invoice)
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
            if (!x.Result)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;

        });
    }

    private async Task<bool> RejectInvoiceImplementationAsync(Invoice invoice, string justification)
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
            if (!x.Result)
            {
                _logger.LogError($"Invoice {invoice.Id}: Approval failed");
                return false;
            }
            return true;

        });
    }

    private async Task<bool> SubmitApprovalImplementationAsync(Invoice invoice)
    {
        var notification = new NotificationBuilder()
                                .WithId(invoice.Id.ToString())
                                .WithScheme(invoice.SchemeType)
                                .WithAction(NotificationType.approval)
                                .WithData(new NotificationOutstandingApproval
                                {
                                    Name = invoice.Approver,
                                    Link = $"{_context.HttpContext.GetBaseURI()}/invoice/details/{invoice.SchemeType}/{invoice.Id.ToString()}/true",
                                    Value = invoice.PaymentRequests.Sum(x => x.Value).ToString(),
                                    InvoiceId = invoice.Id.ToString(),
                                    SchemeType = invoice.SchemeType
                                })
                            .Build();

        return await UpdateAndNotify("approval", invoice, notification).ContinueWith(x =>
        {
            if (!x.Result)
            {
                _logger.LogError($"Invoice {invoice.Id}: Submission failed");
                return false;
            }
            return true;

        });

    }

    private async Task<bool> UpdateAndNotify(string status, Invoice invoice, Notification notification)
    {
        try
        {
            invoice.Status = status;
            invoice.UpdatedBy = "user";
            invoice.Updated = DateTimeOffset.Now;

            var response = await _invoiceAPI.UpdateInvoiceAsync(invoice);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to update invoice");
                foreach (var error in response.Errors)
                {
                    _logger.LogError($"Invoice {invoice.Id}: {error.Key} - {error.Value}");
                }
                return false;
            }
            _logger.LogInformation($"Invoice {invoice.Id}: Updated");

            var addedToQueue = await _queueService.AddMessageToQueueAsync("invoicenotification", notification.ToMessage());
            if (!addedToQueue)
            {
                _logger.LogError($"Invoice {invoice.Id}: Failed to add to queue");
                return false;
            }
            _logger.LogInformation($"Invoice {invoice.Id}: Added to queue");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Invoice {invoice.Id}: {ex.Message}");
            return false;
        }
    }

    private async Task<Invoice> GetApprovalImplementationAsync(string id, string scheme)
    {
        var invoice = await GetInvoiceAsync(id, scheme);

        if (invoice.IsNull())
        {
            _logger.LogError($"Invoice {id}: Not found");
            return null;
        }

        if (invoice.Status != "approval")
        {
            _logger.LogError($"Invoice {invoice.Id}: Not in approval status");
            return null;
        }

        return invoice;
    }

    private async Task<Dictionary<string, string>> GetApproversImplementationAsync()
    {
        var approvers = new Dictionary<string, string>();
        var response = await _approvalAPI.GetApproversAsync("scheme", "value");

        if (response.IsSuccess)
        {
            return (Dictionary<string, string>)response.Data;
        }
        return approvers;
    }

    private async Task<IEnumerable<Invoice>> GetOutstandingApprovalsImplementationAsync()
    {
        var invoices = await _invoiceAPI.GetApprovalsAsync();
        return invoices;
    }
}