namespace EST.MIT.Web.Entities;

public class NotificationInvoiceReject
{
    public string Approver { get; set; } = default!;
    public string Reason { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
}
