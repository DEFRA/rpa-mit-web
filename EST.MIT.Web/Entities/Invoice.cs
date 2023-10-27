using EST.MIT.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EST.MIT.Web.Entities;

namespace Entities;

public class Invoice : Validatable
{
    [Required]
    public Guid Id { get; set; }

    // fxs commented out/changed these attributes
    [Required(ErrorMessage = "Payment Type is required")]
    // [JsonPropertyName("InvoiceType")]
    public string PaymentType { get; set; } = default!;
    [Required(ErrorMessage = "Account Type is required")]
    public string AccountType { get; set; } = default!;
    [Required(ErrorMessage = "Organisation is required")]
    public string Organisation { get; set; } = default!;
    [Required(ErrorMessage = "Scheme Type is required")]
    public string SchemeType { get; set; } = default!;
    public List<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
    public string Status { get; private set; } = "new";
    public string Reference { get; set; } = default!;
    public DateTimeOffset Created { get; private set; } = DateTimeOffset.Now;
    public DateTimeOffset Updated { get; private set; }
    public string CreatedBy { get; private set; } = default!;
    public string UpdatedBy { get; set; } = default!;
    public string Approver { get; set; } = default!;
    public Invoice()
    {
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    public Invoice(Guid id, string paymentType, string accountType, string organisation, string schemeType, List<PaymentRequest> paymentRequests, string status, string reference, DateTimeOffset created, DateTimeOffset updated, string createdBy, string updatedBy, string approver)
    {
        Id = id;
        PaymentType = paymentType;
        AccountType = accountType;
        Organisation = organisation;
        SchemeType = schemeType;
        PaymentRequests = paymentRequests;
        Status = status;
        Reference = reference;
        Created = created;
        Updated = updated;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        Approver = approver;
    }

    public void Update(string? status = null)
    {
        Updated = DateTimeOffset.Now;
        UpdatedBy = "user";
        if (status != null)
        {
            Status = status;
        }
    }
    public override Dictionary<string, List<string>> AddErrors(Dictionary<string, List<string>> errors)
    {
        for (int paymentRequestIndex = 0; paymentRequestIndex < PaymentRequests.Count; paymentRequestIndex++)
        {
            PaymentRequest paymentRequest = PaymentRequests[paymentRequestIndex];
            paymentRequest.ErrorPath = string.Concat(ErrorPath, $"{nameof(PaymentRequest)}s[", paymentRequestIndex, "].");
            paymentRequest.AddErrors(errors);
        }
        return base.AddErrors(errors);
    }
}