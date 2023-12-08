namespace EST.MIT.Web.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    public string Email { get; set; }

    [JsonIgnore]
    public string StatusTag => GetStatusTag();
    private string GetStatusTag()
    {
        return (Status) switch
        {
            UploadStatus.Upload_successful => "govuk-tag--blue",
            UploadStatus.Upload_failed => "govuk-tag--red",
            UploadStatus.Upload_validated => "govuk-tag--yellow",
            UploadStatus.Uploaded => "govuk-tag--green",
            _ => null
        };
    }
}

public static class ImportRequestExtentions
{
    public static string ToMessage(this ImportRequest entity) => JsonSerializer.Serialize<ImportRequest>(entity);
}
