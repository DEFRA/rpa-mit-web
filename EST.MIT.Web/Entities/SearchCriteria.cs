using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.Entities;

[ExcludeFromCodeCoverage]
public class SearchCriteria : IValidatableObject
{
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

    public string InvoiceNumber { get; set; } = "";

    public string PaymentRequestId { get; set; } = "";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var populatedFields = new List<string>();

        if (!string.IsNullOrWhiteSpace(InvoiceNumber)) populatedFields.Add(nameof(InvoiceNumber));
        if (!string.IsNullOrWhiteSpace(PaymentRequestId)) populatedFields.Add(nameof(PaymentRequestId));

        if (populatedFields.Count == 0)
        {
            var errorMessage = "An Invoice ID or Payment Requiest ID is required";
            yield return new ValidationResult(errorMessage, new[] { "Search Criteria" });
        }
        else if (populatedFields.Count > 1)
        {
            foreach (var field in populatedFields)
            {
                yield return new ValidationResult($"Only one of Invoice ID or Payment Requiest ID can be entered", new[] { field });
            }
        }
    }
}