using System.ComponentModel;
using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Attributes;

namespace EST.MIT.Web.Entities;

public class PaymentRequest : Validatable
{
    public string PaymentRequestId { get; set; }
    [Required]
    public string SourceSystem { get; set; } = "Manual";
    [Required]
    [RegularExpression("(\\d{10})", ErrorMessage = "The FRN must be 10 digits")]
    public int FRN { get; set; }
    [Range(2014, int.MaxValue, ErrorMessage = "The Marketing Year must be after than 2014")]
    public int MarketingYear { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The Payment Request Number must be greater than 0")]
    public int PaymentRequestNumber { get; set; }
    //Invoice Number
    [Required(ErrorMessage = "The Agreement Number is required")]
    public string AgreementNumber { get; set; } = string.Empty;
    [Required]
    [RegularExpression("GBP|EUR", ErrorMessage = "The Currency must be either GBP or EUR")]
    public string Currency { get; set; } = "GBP";
    public string DueDate { get; set; } = string.Empty;
    [Required]
    [RegularExpression("(^(0|\\d+\\.\\d{2})$)", ErrorMessage = "The Value must be in the format 0.00")]
    public double Value { get; set; } = 0.00;
    public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    public string AccountType { get; set; } = string.Empty;

    [RequiredIfAR]
    [DisplayName("Original Claim Reference")]
    public string OriginalInvoiceNumber { get; set; } = string.Empty;

    [RequiredIfAR]
    [DisplayName("Original AP Invoice Settlement Date")]
    public DateTimeOffset OriginalSettlementDate { get; set; } = default!;

    [RequiredIfAR]
    [DisplayName("Earliest date possible recovery first identified")]
    public DateTimeOffset RecoveryDate { get; set; } = default!;

    [RequiredIfAR]
    [DisplayName("Correction Reference - Previous AR Invoice ID")]
    public string InvoiceCorrectionReference { get; set; } = string.Empty;
    public override Dictionary<string, List<string>> AddErrors(Dictionary<string, List<string>> errors)
    {
        for (int invoiceLineIndex = 0; invoiceLineIndex < InvoiceLines.Count; invoiceLineIndex++)
        {
            InvoiceLine invoiceLine = InvoiceLines[invoiceLineIndex];
            invoiceLine.ErrorPath = string.Concat(ErrorPath, $"{nameof(PaymentRequest.InvoiceLines)}[", invoiceLineIndex, "].");
            invoiceLine.AddErrors(errors);
        }
        return base.AddErrors(errors);
    }
}
