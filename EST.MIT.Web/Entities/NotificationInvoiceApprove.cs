namespace EST.MIT.Web.Entities;

public class NotificationInvoiceApprove
{
    public string ApproverEmail { get; set; } = default!;
    public string Link { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string InvoiceId { get; set; } = default!;
    public string SchemeType { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
}
