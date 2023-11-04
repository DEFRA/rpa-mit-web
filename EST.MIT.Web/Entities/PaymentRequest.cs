using System.ComponentModel;
using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using EST.MIT.Web.Attributes;
using Newtonsoft.Json;

namespace EST.MIT.Web.Entities;

public class PaymentRequest : Validatable, IValidatableObject
{
    public PaymentRequest()
    {
        this.PaymentRequestNumber = 1;
        this.DueDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");

        // TODO: fxs, Remove these defaults, helps with debugging
        //this.FRN = "1234567890";
        //this.MarketingYear = "2015";
        //this.AgreementNumber = "1";
        //this.Currency = "GBP";
        //this.Value = 0;
        //this.OriginalInvoiceNumber = "1";
        //this.OriginalSettlementDate = DateTime.Now;
        //this.RecoveryDate = DateTime.Now;
        //this.InvoiceCorrectionReference = "1";
        //this.Description = "DescriptionValue";
    }

    public string PaymentRequestId { get; set; }

    [Required]
    public string SourceSystem { get; set; } = "Manual";

    [DisplayName("FRN")]
    [RegularExpression(@"^([0-9]{10})?$", ErrorMessage = "The FRN must be a 10-digit number or be empty.")]
    public string FRN { get; set; }

    [Required]
    [RegularExpression(@"^(201[5-9]|20[2-9]\d|[2-9]\d{3})$", ErrorMessage = "The Marketing Year must be after than 2014")]
    public string MarketingYear { get; set; }


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
    public DateTime OriginalSettlementDate { get; set; } = default!;

    [RequiredIfAR]
    [DisplayName("Earliest date possible recovery first identified")]
    public DateTime RecoveryDate { get; set; } = default!;

    [RequiredIfAR]
    [DisplayName("Correction Reference - Previous AR Invoice ID")]
    public string InvoiceCorrectionReference { get; set; } = string.Empty;

    [DisplayName("SBI")]
    [RegularExpression(@"^((105000000|1[0-9]{8}|[2-9][0-9]{8}))?$", ErrorMessage = "The SBI is not in valid range (105000000 .. 999999999) or should be empty.")]
    public string SBI { get; set; }

    [DisplayName("Vendor")]
    [RegularExpression(@"^.{6}$|^$", ErrorMessage = "The Vendor must be 6 characters or be empty.")]
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
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var populatedFields = new List<string>();

        if (!string.IsNullOrWhiteSpace(FRN)) populatedFields.Add(nameof(FRN));
        if (!string.IsNullOrWhiteSpace(SBI)) populatedFields.Add(nameof(SBI));
        if (!string.IsNullOrWhiteSpace(Vendor)) populatedFields.Add(nameof(Vendor));

        if (populatedFields.Count == 0)
        {
            var errorMessage = "At least one of FRN, SBI, or Vendor must be entered";
            yield return new ValidationResult(errorMessage, new[] { "CustomerReference" });
        }
        else if (populatedFields.Count > 1)
        {
            // Adding specific errors for each of the populated fields
            foreach (var field in populatedFields)
            {
                yield return new ValidationResult($"Only one of FRN, SBI, or Vendor can be entered", new[] { field });
            }
        }
    }
}
