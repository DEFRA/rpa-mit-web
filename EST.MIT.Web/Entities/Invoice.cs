using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities;

public class Invoice
{
    [Required]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Invoice Type is required")]
    [JsonPropertyName("InvoiceType")]
    public string PaymentType { get; set; } = default!;
    [Required(ErrorMessage = "Account Type is required")]
    public string AccountType { get; set; } = default!;
    [Required(ErrorMessage = "Organisation is required")]
    public string Organisation { get; set; } = default!;
    [Required(ErrorMessage = "Scheme Type is required")]
    public string SchemeType { get; set; } = default!;
    public List<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
    public string Status { get; set; } = "new";
    public string Reference { get; set; } = default!;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public string CreatedBy { get; set; } = "";
    public string UpdatedBy { get; set; } = "";
    public string Approver { get; set; } = "";

    public Invoice()
    {
        Id = Guid.NewGuid();
    }
}