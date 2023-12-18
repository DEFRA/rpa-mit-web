namespace EST.MIT.Web.Models;

public static class InvoiceStatuses
{
    public static readonly string New = "new";
    public static readonly string AwaitingApproval = "AWAITING_APPROVAL";
    public static readonly string Approved = "APPROVED";
    public static readonly string Rejected = "REJECTED";

    public static string DisplayNameFor(string status)
    {
        if (status == "new")
        {
            return "New";
        }

        if (status == "AWAITING_APPROVAL")
        {
            return "Awaiting Approval";
        }

        if (status == "APPROVED")
        {
            return "Approved";
        }

        if (status == "REJECTED")
        {
            return "Rejected";
        }
        return status;
    }
}