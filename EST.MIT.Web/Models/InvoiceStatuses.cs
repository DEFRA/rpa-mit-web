namespace EST.MIT.Web.Models;

public static class InvoiceStatuses
{
    public static readonly string New = "new";
    public static readonly string AwaitingApproval = "AWAITING_APPROVAL";
    public static readonly string Approved = "APPROVED";
    public static readonly string Rejected = "REJECTED";

    public static string DisplayNameFor(string status)
    {
        if (status == New)
        {
            return "New";
        }

        if (status == AwaitingApproval)
        {
            return "Awaiting Approval";
        }

        if (status == Approved)
        {
            return "Approved";
        }

        if (status == Rejected)
        {
            return "Rejected";
        }
        return status;
    }
}