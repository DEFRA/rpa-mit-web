using EST.MIT.Web.Helpers;
using EST.MIT.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EST.MIT.Web.Entities;

public class Invoice : Validatable
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Payment Type is required")]
    public string PaymentType { get; set; } = default!;

    [Required(ErrorMessage = "Account Type is required")]
    public string AccountType { get; set; } = default!;

    [Required(ErrorMessage = "Organisation is required")]
    public string Organisation { get; set; } = default!;

    [Required(ErrorMessage = "Scheme Type is required")]
    public string SchemeType { get; set; } = default!;

    public List<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();

    public string Status { get; private set; } = InvoiceStatuses.New;

    public string Reference { get; set; } = default!;

    public DateTimeOffset Created { get; private set; } = DateTimeOffset.Now;

    public DateTimeOffset Updated { get; private set; }

    public string ApproverId { get; set; } = default!;
    public string ApproverEmail { get; set; } = default!;
    public string ApprovalRequestedByEmail { get; set; } = default!;
    public string ApprovedBy { get; set; } = default!;
    public DateTime? Approved { get; set; }

    public string CreatedBy { get; private set; } = default!;

    public string UpdatedBy { get; set; } = default!;

    [JsonIgnore]
    public int NumberOfPaymentRequests => PaymentRequests.Count;
    [JsonIgnore]
    public decimal TotalValueOfPaymentsGBP => PaymentRequests.Where(x => x.Currency == "GBP").Sum(x => x.Value);
    [JsonIgnore]
    public decimal TotalValueOfPaymentsEUR => PaymentRequests.Where(x => x.Currency == "EUR").Sum(x => x.Value);
    [JsonIgnore]
    public bool CanBeSentForApproval => Status == InvoiceStatuses.New && PaymentRequests.All(x => x.Value != 0 && x.InvoiceLines.Count != 0);

    public string ApprovalGroup
    {
        get
        {
            if (Organisation == "RPA")
            {
                return SchemeType;
            }
            return Organisation;
        }
    }

    public Invoice()
    {
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    public Invoice(Guid id, string paymentType, string accountType, string organisation, string schemeType, List<PaymentRequest> paymentRequests, string status, string reference, DateTimeOffset created, DateTimeOffset updated, string createdBy, string updatedBy, string approverId, string approverEmail, string approvedBy, DateTime approved)
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
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        Updated = updated;
        ApproverId = approverId;
        ApproverEmail = approverEmail;
        ApprovedBy = approvedBy;
        Approved = approved;
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
            paymentRequest.ErrorPath = $"{nameof(Invoice.PaymentRequests)}[{paymentRequestIndex}]";
            paymentRequest.AddErrors(errors);
        }
        return base.AddErrors(errors);
    }

    public override Dictionary<string, List<string>> AllErrors
    {
        get
        {
            Dictionary<string, List<string>> allErrors = Errors;
            foreach (var paymentRequest in PaymentRequests)
            {
                foreach (var error in paymentRequest.Errors)
                {
                    if (!allErrors.ContainsKey(error.Key))
                    {
                        allErrors.Add(error.Key, error.Value);
                    }
                }
            }
            return allErrors;
        }
    }
}