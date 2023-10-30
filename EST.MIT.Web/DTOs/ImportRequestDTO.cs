using EST.MIT.Web.Entities;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.DTOs;

[ExcludeFromCodeCoverage]
public class ImportRequestDTO
{
    [JsonProperty("paymentRequestId")]
    public string PaymentRequestId { get; init; } = default!;

    [JsonProperty("importRequestId")]
    public Guid ImportRequestId { get; set; }

    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("fileSize")]
    public int FileSize { get; set; }

    [JsonProperty("fileType")]
    public string FileType { get; set; }

    [JsonProperty("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonProperty("paymentType")]
    public string PaymentType { get; set; }

    [JsonProperty("organisation")]
    public string Organisation { get; set; }

    [JsonProperty("schemeType")]
    public string SchemeType { get; set; }

    [JsonProperty("accountType")]
    public string AccountType { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; init; }

    [JsonProperty("status")]
    public UploadStatus Status { get; init; }

    [JsonProperty("blobPath")]
    public string BlobPath { get; set; }
}