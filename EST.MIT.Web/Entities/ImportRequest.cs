namespace EST.MIT.Web.Entities;
using System.Text.Json;

public class ImportRequest
{
    public Guid ImportRequestId { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string FileType { get; set; }
    public DateTimeOffset? Timestamp { get; set; }

    public string PaymentType { get; set; }
    public string Organisation { get; set; }
    public string SchemeType { get; set; }
    public string AccountType { get; set; }
    public string CreatedBy { get; init; }
    public UploadStatus Status { get; init; }
    public string BlobFileName { get; set; }
    public string BlobFolder { get; set; }
}

public static class ImportRequestExtentions
{
    public static string ToMessage(this ImportRequest entity) => JsonSerializer.Serialize<ImportRequest>(entity);
}
