using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities;

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
    public override Dictionary<string, string> AddErrors(Dictionary<string, string> errors)
    {
        for (int invoiceLineIndex = 0; invoiceLineIndex < InvoiceLines.Count; invoiceLineIndex++)
        {
            InvoiceLine invoiceLine = InvoiceLines[invoiceLineIndex];
            invoiceLine.ErrorPath = string.Concat(ErrorPath, "InvoiceLines[", invoiceLineIndex, "].");
            invoiceLine.AddErrors(errors);
        }
        return base.AddErrors(errors);
    }
}
