using System.ComponentModel;
using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Attributes;
using Newtonsoft.Json;

namespace EST.MIT.Web.Entities;

public class PaymentRequest : Validatable
{
    public PaymentRequest()
    {
        this.PaymentRequestNumber = 1;
    }

    public string PaymentRequestId { get; set; }

    [Required]
    public string SourceSystem { get; set; } = "Manual";

    [DisplayName("FRN")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "The FRN must be a valid number")]
    [MaxLength(10, ErrorMessage = "The FRN must be 10 characters")]
    [MinLength(10, ErrorMessage = "The FRN must be 10 characters")]
    public string FRN { get; set; }
    

    [Range(2014, int.MaxValue, ErrorMessage = "The Marketing Year must be after than 2014")]
    public int MarketingYear { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The Payment Request Number must be greater than 0")]
    public int PaymentRequestNumber { get; set; }

    [Required(ErrorMessage = "The Agreement Number is required")]
    public string AgreementNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression("GBP|EUR", ErrorMessage = "The Currency must be either GBP or EUR")]
    public string Currency { get; set; } = "GBP";

    public string DueDate { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[0-9]*(\.[0-9]{1,2})?$", ErrorMessage = "The value must be valid number and have a maximum of 2 decimal places.")]
    public decimal Value { get; set; }

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

    [DisplayName("SBI")]
    [RegularExpression(@"^(105000000|1[0-9]{8}|[2-9][0-9]{8})$", ErrorMessage = "The SBI is not in valid range (105000000 .. 999999999)")]
    [MaxLength(9, ErrorMessage = "The SBI must be 9 characters")]
    [MinLength(9, ErrorMessage = "The SBI must be 9 characters")]
    public string SBI { get; set; }

    [DisplayName("Vendor")]
    [MaxLength(6, ErrorMessage = "The Vendor must be 6 characters")]
    [MinLength(6, ErrorMessage = "The Vendor must be 6 characters")]
    public string Vendor { get; set; } = string.Empty;

    [Required]
    [DisplayName("Description")]
    public string Description { get; set; } = string.Empty;

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
