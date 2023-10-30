namespace EST.MIT.Web.Entities;

public class ImportRequest
{
    public Guid ImportRequestId { get; set; }
    public string FileName { get; set; }
    public int FileSize { get; set; }
    public string FileType { get; set; }
    public DateTimeOffset? Timestamp { get; set; }

    public string PaymentType { get; set; }
    public string Organisation { get; set; }
    public string SchemeType { get; set; }
    public string AccountType { get; set; }
    public string CreatedBy { get; init; }
    public UploadStatus Status { get; init; }
    public string BlobPath { get; set; }
}
