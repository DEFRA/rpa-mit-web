using System.ComponentModel.DataAnnotations;
using Entities;
using EST.MIT.Web.Attributes;

namespace EST.MIT.Web.Entities;

public class PaymentRequest
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

    [RequiredIfAR]
    public string OriginalInvoiceNumber { get; set; } = string.Empty;

    [RequiredIfAR]
    public DateTime OriginalSettlementDate { get; set; } = default!;

[RequiredIfAR]
	public DateTime RecoveryDate { get; set; } = default!;

	[RequiredIfAR]
    public string InvoiceCorrectionReference { get; set; } = string.Empty;
}
