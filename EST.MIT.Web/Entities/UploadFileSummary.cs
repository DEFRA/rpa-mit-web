using System.Text.Json;

namespace EST.MIT.Web.Entities;

public class UploadFileSummary
{
    public string FileName { get; set; } = default!;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string ConfirmationNumber { get; }
    public string AccountType { get; set; }
    public string Organisation { get; set; }
    public string SchemeType { get; set; }
    public string PaymentType { get; set; }
    public string UserID { get; set; }

    public UploadFileSummary(string confirmationNumber)
    {
        ConfirmationNumber = confirmationNumber;
    }

}

public static class UploadFileSummaryExtentions
{
    public static string ToMessage(this UploadFileSummary entity) => JsonSerializer.Serialize<UploadFileSummary>(entity);
}
