namespace EST.MIT.Web.Entities;

public class NotificationInvoiceApprove
{
    public string ApproverEmail { get; set; } = default!;
    public string CreatorEmail { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
}
