using System.Text.Json;

namespace Entities;

public class UploadFileSummary
{
    public string FileName { get; set; } = default!;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string ConfirmationNumber { get; }

    public UploadFileSummary(string confirmationNumber)
    {
        ConfirmationNumber = confirmationNumber;
    }

}

public static class UploadFileSummaryExtentions
{
    public static string ToMessage(this UploadFileSummary entity) => JsonSerializer.Serialize<UploadFileSummary>(entity);
}
