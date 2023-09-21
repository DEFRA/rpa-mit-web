using System.ComponentModel.DataAnnotations;

namespace Entities;

public class PaymentRequest
{
    public string PaymentRequestId { get; set; }
    public string AccountType { get; set; }
    [Required]
    public string ClaimReferenceNumber { get; set; }
    public string ClaimReference { get; set; }
    [Required]
    public string CustomerId { get; set; }
    public double Value { get; set; } = 0.00;
    [Required]
    [RegularExpression("GBP|EUR", ErrorMessage = "The Currency must be either GBP or EUR")]
    public string Currency { get; set; } = "GBP";
    public string Description { get; set; }
    [RequiredIfAR]
    public string OriginalClaimReference { get; set; }
    [RequiredIfAR]
    public string OriginalSettlementDate { get; set; }
    [RequiredIfAR]
    public string RecoveryIdentified { get; set; }
    [RequiredIfAR]
    public string InvoiceCorrectionReference { get; set; }
    [Required]
    public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

}