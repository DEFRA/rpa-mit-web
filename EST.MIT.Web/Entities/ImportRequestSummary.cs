namespace EST.MIT.Web.Entities;

public class ImportRequestSummary
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
    public string CreatedBy { get; set; }

    public ImportRequestSummary(ImportRequest importRequest, string confirmationNumber)
    {
        FileName = importRequest.FileName;
        FileSize = importRequest.FileSize;
        FileType = importRequest.FileType;
        Timestamp = importRequest.Timestamp ?? DateTimeOffset.Now;
        ConfirmationNumber = confirmationNumber;
        AccountType = importRequest.AccountType;
        Organisation = importRequest.Organisation;
        SchemeType = importRequest.SchemeType;
        PaymentType = importRequest.PaymentType;
        CreatedBy = importRequest.CreatedBy;
    }
}
