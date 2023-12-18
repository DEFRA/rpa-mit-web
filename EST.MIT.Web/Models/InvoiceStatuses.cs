namespace EST.MIT.Web.Models;

public static class InvoiceStatuses
{
    public static readonly string New = "new";
    public static readonly string AwaitingApproval = "AWAITING_APPROVAL";
    public static readonly string Approved = "APPROVED";
    public static readonly string Rejected = "REJECTED";


    public static string DisplayNameFor(string status)
    {
        return status switch
        {
            "new" => New,
            "AwaitingApproval" => "Awaiting approval",
            "Approved" => Approved,
            "Rejected" => Rejected,
            _ => status,
        };
    }
}